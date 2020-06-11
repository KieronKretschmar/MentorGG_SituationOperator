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
    public class DeathInducedBombDropManager : SinglePlayerSituationManager<DeathInducedBombDrop>
    {
        /// <summary>
        /// Minimum required teammates alive at death to count as a misplay.
        /// </summary>
        private const int MIN_TEAMMATES_ALIVE = 2;

        /// <summary>
        /// Allowed time for a teammate to die before the bomb was dropped to count as alive.
        /// </summary>
        private const int MAX_TIME_TEAMMATE_DIED_BEFORE_TO_COUNT_AS_ALIVE = 4000;

        /// <summary>
        /// Minimum required distance in meters to the nearest teammate at death count as a misplay.
        /// </summary>
        private const double MIN_TEAMMATE_DISTANCE = 2;

        /// <summary>
        /// Minimum time the player must have held the bomb before dying to count as a misplay.
        /// </summary>
        private const double MIN_TIME_HELD_BOMB = 6000;

        /// <summary>
        /// Whether no teammate may have stood "between" killer and player to count as a misplay.
        /// 
        /// Nobody being "between" player and killer means that nobody was 
        /// - closer to the player than the killer, AND
        /// - closer to the killer than the player.
        /// </summary>
        private const bool REQUIRE_NO_TEAMMATE_BETWEEN_KILLER_AND_PLAYER = true;

        private readonly IServiceProvider _sp;
        private readonly ILogger<DeathInducedBombDropManager> _logger;

        /// <summary>
        /// Constructor.
        /// </summary>
        public DeathInducedBombDropManager(IServiceProvider sp, ILogger<DeathInducedBombDropManager> logger, SituationContext context) : base(context)
        {
            _sp = sp;
            _logger = logger;
        }

        /// <inheritdoc/>
        public override SituationCategory SituationCategory => SituationCategory.Misplay;

        /// <inheritdoc/>
        public override SkillDomain SkillDomain => SkillDomain.Tactical;

        /// <inheritdoc/>
        public override SituationType SituationType => SituationType.DeathInducedBombDrop;

        /// <inheritdoc/>
        protected override Func<SituationContext, DbSet<DeathInducedBombDrop>> TableSelector => context => context.DeathInducedBombDrop;

        /// <inheritdoc/>
        protected override async Task<IEnumerable<DeathInducedBombDrop>> ExtractSituationsAsync(MatchDataSet data)
        {
            using (var scope = _sp.CreateScope())
            {
                var misplays = new List<DeathInducedBombDrop>();
                var bombDrops = data.ItemDroppedList
                    .Where(x => x.Equipment == EquipmentElement.Bomb)
                    // ItemDropped.ByDeath is wrong in database as of 13.09.2019. Remove the '!' when DemoAnalyzer is fixed
                    .Where(x => !x.ByDeath)
                    // Remove bombdrops of bots who died due to takeover
                    .Where(x=>(x.PlayerId < 0 && data.BotTakeOverList.Any(takover => takover.Time == x.Time)) == false)
                    .ToList();

                foreach (var bombDrop in bombDrops)
                {
                    var obtainedBombTime = data.ObtainedItemTime(bombDrop.PlayerId, bombDrop.Round, EquipmentElement.Bomb, bombDrop.Time);
                    if (bombDrop.Time - MIN_TIME_HELD_BOMB < obtainedBombTime)
                        continue;

                    // Get SteamIds of teammates who lived up until a few moments before the player died
                    var involvedTeammates = data.GetTeammateRoundStats(bombDrop.PlayerId, bombDrop.Round)
                        .Where(x => x.PlayerId != bombDrop.PlayerId)
                        .Select(x=> new
                        {
                            PlayerId = x.PlayerId,
                            IsInvolved = data.IsAlive(x.PlayerId, bombDrop.Round, bombDrop.Time - MAX_TIME_TEAMMATE_DIED_BEFORE_TO_COUNT_AS_ALIVE),
                            IsAliveAtBombDrop = data.IsAlive(x.PlayerId, bombDrop.Round, bombDrop.Time)
                        })
                        .Where(x => x.IsInvolved);                       

                    var teammatesAlive = involvedTeammates.Count(x=>x.IsAliveAtBombDrop);

                    // Ignore bombdrops where too few teammates were alive
                    if (teammatesAlive < MIN_TEAMMATES_ALIVE)
                    {
                        continue;
                    }

                    var lastVictimPosition = data.LastPlayerPos(bombDrop.PlayerId, bombDrop.Time);
                    var teammatePositions = involvedTeammates
                        .ToDictionary(x => x, x => new
                        {
                            LastPosition = data.LastPlayerPos(x.PlayerId, bombDrop.Time).PlayerPos
                        })
                        .ToDictionary(x => x.Key, x => new
                        {
                            x.Value.LastPosition,
                            LastDistanceToVictim = Vector3.Distance(lastVictimPosition.PlayerPos, x.Value.LastPosition)
                        });

                    var closestTeammateDistance = teammatePositions.Select(x=>x.Value.LastDistanceToVictim).Min();
                    if(closestTeammateDistance < UnitConverter.MetersToUnits(MIN_TEAMMATE_DISTANCE))
                    {
                        continue;
                    }

                    if (REQUIRE_NO_TEAMMATE_BETWEEN_KILLER_AND_PLAYER)
                    {
                        var teammateBetweenKillerAndVictim = false;

                        var kill = data.Death(bombDrop.PlayerId, bombDrop.Round);
                        var killerPosition = data.LastPlayerPos(kill.PlayerId, kill.Time);
                        var victimToKillerDistance = Vector3.Distance(killerPosition.PlayerPos, lastVictimPosition.PlayerPos);

                        foreach (var teammatePosition in teammatePositions)
                        {
                            var teammateToKillerDistance = Vector3.Distance(teammatePosition.Value.LastPosition, killerPosition.PlayerPos);
                            var teammateToVictimDistance = teammatePosition.Value.LastDistanceToVictim;
                            var teammateWasBetweenVictimAndKiller = teammateToVictimDistance <= victimToKillerDistance && teammateToKillerDistance <= victimToKillerDistance;
                            if (teammateWasBetweenVictimAndKiller)
                            {
                                teammateBetweenKillerAndVictim = true;
                                break;
                            }
                        }

                        if (teammateBetweenKillerAndVictim)
                            continue;
                    }

                    var pickedUpAfter = data.ItemPickedUpList
                        .FirstOrDefault(x =>
                            x.Equipment == EquipmentElement.Bomb
                            && x.Round == bombDrop.Round
                            && x.Time > bombDrop.Time
                        )?.Time - bombDrop.Time ?? -1;

                    var misplay = new DeathInducedBombDrop(bombDrop, pickedUpAfter, teammatesAlive, (float) closestTeammateDistance);

                    misplays.Add(misplay);
                }

                return misplays;
            }
        }
    }
}
