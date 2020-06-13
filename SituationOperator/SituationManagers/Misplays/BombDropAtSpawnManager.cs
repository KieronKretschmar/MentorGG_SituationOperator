using MatchEntities;
using MatchEntities.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SituationDatabase;
using SituationDatabase.Enums;
using SituationDatabase.Models;
using SituationOperator.Enums;
using SituationOperator.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using ZoneReader;

namespace SituationOperator.SituationManagers
{
    /// <summary>
    /// A SituationManager. 
    /// See <see cref="ExtractSituationsAsync(MatchDataSet)"/> for more info regarding Situation specific logic.
    /// </summary>
    public class BombDropAtSpawnManager : SinglePlayerSituationManager<BombDropAtSpawn>
    {
        /// <summary>
        /// Maximum required time at which the bomb must have been dropped after freezetime ended to count as a misplay.
        /// </summary>
        private const int MAX_TIME_DROPPED_AFTER_FREEZETIME_END = 4000;

        /// <summary>
        /// Minimum required value in meters that each teammate must have increased their distance from the bomb from the moment it dropped until pickup to count as a misplay.
        /// </summary>
        private const double MIN_DISTANCE_TEAMMATES_DETOURED = 0.5;

        /// <summary>
        /// Minimum required time the bomb must have been on the ground after freezetime ended to count as a misplay.
        /// </summary>
        private const int MIN_TIME_ON_GROUND_AFTER_FREEZETIME_END = 1000;
        
        /// <summary>
        /// Whether the player picking up the bomb may not have stood still before picking up the bomb to count as a misplay.
        /// 
        /// Reason: To prevent planned situations counting as a misplay where e.g. a Terrorist on de_mirage scopes through middle and then picks up the bomb.
        /// </summary>
        private const bool REQUIRE_PICKER_DID_NOT_STOP = true;

        private readonly IServiceProvider _sp;
        private readonly ILogger<BombDropAtSpawnManager> _logger;

        /// <summary>
        /// Constructor.
        /// </summary>
        public BombDropAtSpawnManager(IServiceProvider sp, ILogger<BombDropAtSpawnManager> logger, SituationContext context) : base(context)
        {
            _sp = sp;
            _logger = logger;
        }

        /// <inheritdoc/>
        public override SituationCategory SituationCategory => SituationCategory.Misplay;

        /// <inheritdoc/>
        public override SkillDomain SkillDomain => SkillDomain.Tactical;

        /// <inheritdoc/>
        public override SituationType SituationType => SituationType.BombDropAtSpawn;

        /// <inheritdoc/>
        protected override Func<SituationContext, DbSet<BombDropAtSpawn>> TableSelector => context => context.BombDropAtSpawn;

        /// <inheritdoc/>
        protected override async Task<IEnumerable<BombDropAtSpawn>> ExtractSituationsAsync(MatchDataSet data)
        {
            using (var scope = _sp.CreateScope())
            {
                var bombDrops = data.ItemDroppedList
                    .Where(x => x.Equipment == EquipmentElement.Bomb)
                    .ToList();

                var misplays = new List<BombDropAtSpawn>();
                foreach (var bombDrop in bombDrops)
                {
                    var round = data.GetRoundStats(bombDrop);

                    // Filter out bombdrops that did not happen at roundstart
                    var timerSettings = data.GetMatchSettings();
                    var freezeTimeEnd = round.StartTime + timerSettings.FreezeTime;
                    if (freezeTimeEnd + MAX_TIME_DROPPED_AFTER_FREEZETIME_END < bombDrop.Time)
                        continue;

                    // Get first pickup after bombDrop
                    var pickUp = data.ItemPickedUpList
                        .Where(x => x.Equipment == EquipmentElement.Bomb)
                        .Where(x => x.Round == bombDrop.Round)
                        .Where(x => bombDrop.Time < x.Time)
                        .OrderBy(x=>x.Time)
                        .FirstOrDefault();

                    // If the bomb was not picked up again, it's a misplay
                    if(pickUp == null)
                    {
                        misplays.Add(new BombDropAtSpawn(bombDrop, -1));
                        continue;
                    }

                    #region DetourCondition
                    // Estimate the bomb location by using the last known location of the dropper, 
                    // as th exact location is currently not stored in BombDrop entity
                    // See https://gitlab.com/mentorgg/csgo/demofileworker/-/issues/15
                    var estimatedBombLocation = data.LastPlayerPos(bombDrop.PlayerId, bombDrop.Time).PlayerPos;

                    // Check whether the player picking up the bomb did not stop and whether all teammates have moved away from the bomb at least once
                    var pickerDidNotStopConditionHolds = true;
                    // start with a true value and set to false as soon as a player without detour was detected
                    var allTeammatesDetouredConditionHolds = true;
                    var teammateSteamIds = data.GetTeammateRoundStats(bombDrop.PlayerId, bombDrop.Round)
                        .Select(x=>x.PlayerId)
                        .Where(x=>x != bombDrop.PlayerId);
                    foreach (var steamId in teammateSteamIds)
                    {
                        // Get this players positions between drop and pickup
                        var positionsWhileBombOnGround = data.PlayerPositionList
                            .Where(x => x.Round == bombDrop.Round && x.PlayerId == steamId)
                            .Where(x => bombDrop.Time <= x.Time && x.Time <= pickUp.Time)
                            .ToList();

                        // Check REQUIRE_PICKER_DID_NOT_STOP if required
                        if (REQUIRE_PICKER_DID_NOT_STOP && steamId == pickUp.PlayerId)
                        {
                            if(positionsWhileBombOnGround.Any(x => x.PlayerVelo.Length() == 0))
                            {
                                pickerDidNotStopConditionHolds = false;
                                break;
                            }
                        }

                        // Add last known position before the bombDrop to list because frames can be missing in dataset if he stood still
                        positionsWhileBombOnGround.Insert(0, data.LastPlayerPos(steamId, bombDrop.Time));

                        // Ignore player if they didn't move at all (AFK / dead / kicked) 
                        if (positionsWhileBombOnGround.Count <= 1)
                            continue;

                        var distances = positionsWhileBombOnGround.Select(x => Vector3.Distance(estimatedBombLocation, x.PlayerPos));
                        var initialDistance = distances.First();
                        var maxDistance = distances.Max();

                        if (maxDistance - initialDistance < UnitConverter.MetersToUnits(MIN_DISTANCE_TEAMMATES_DETOURED))
                        {
                            allTeammatesDetouredConditionHolds = false;
                            break;
                        }
                    }

                    if(REQUIRE_PICKER_DID_NOT_STOP && pickerDidNotStopConditionHolds == false)
                    {
                        _logger.LogDebug($"REQUIRE_PICKER_DID_NOT_STOP condition did not hold for bombdrop in match [ {bombDrop.MatchId} ] and round [ {bombDrop.Round} ]");
                        continue;
                    }

                    if (allTeammatesDetouredConditionHolds == false)
                    {
                        _logger.LogDebug($"MIN_DISTANCE_TEAMMATES_DETOURED condition did not hold for bombdrop in match [ {bombDrop.MatchId} ] and round [ {bombDrop.Round} ]");
                        continue;
                    }
                    #endregion

                    var pickedUpAfter = pickUp.Time - bombDrop.Time;
                    var misplay = new BombDropAtSpawn(bombDrop, pickedUpAfter);
                    misplays.Add(misplay);
                }

                return misplays;
            }
        }
    }
}
