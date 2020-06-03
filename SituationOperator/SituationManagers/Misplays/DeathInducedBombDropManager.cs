﻿using MatchEntities;
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
        private const int MIN_TEAMMATES_ALIVE = 1;

        /// <summary>
        /// Minimum required distance (csgo units) to the nearest teammate at death count as a misplay.
        /// 
        /// 1 Meter ~ 525 units (see https://developer.valvesoftware.com/wiki/Dimensions#Map_Grid_Units:_quick_reference)
        /// </summary>
        private const int MIN_TEAMMATE_DISTANCE = 300;

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
                    .Where(x => x.MatchId == data.MatchId && x.Equipment == EquipmentElement.Bomb)
                    // ItemDropped.ByDeath is wrong in database as of 13.09.2019. Remove the '!' when DemoAnalyzer is fixed
                    .Where(x => !x.ByDeath)
                    .ToList();

                foreach (var bombDrop in bombDrops)
                {

                    var livingTeammateSteamIds = TeammateSteamIds(data, bombDrop.PlayerId)
                        .Where(x => data.IsAlive(x, bombDrop.Time));


                    var teamMateDistances = livingTeammateSteamIds
                        .Select(x => data.LastPositionDistance(bombDrop.PlayerId, x, bombDrop.Time));


                    var teammatesAlive = livingTeammateSteamIds.Count();

                    // Ignore bombdrops where too few teammates were alive
                    if (teammatesAlive < MIN_TEAMMATES_ALIVE)
                    {
                        continue;
                    }

                    var closestTeammateDistance = teamMateDistances.Min() ?? null; // -1 if none alive

                    if(closestTeammateDistance == null || closestTeammateDistance < MIN_TEAMMATE_DISTANCE)
                    {
                        continue;
                    }

                    var pickedUpAfter = data.ItemPickedUpList
                        .FirstOrDefault(x =>
                            x.Equipment == EquipmentElement.Bomb
                            && x.MatchId == bombDrop.MatchId
                            && x.Round == bombDrop.Round
                            && x.Time > bombDrop.Time
                        )?.Time - bombDrop.Time ?? -1;

                    var misplay = new DeathInducedBombDrop(bombDrop, pickedUpAfter, teammatesAlive, (float) closestTeammateDistance);

                    misplays.Add(misplay);

                }

                return misplays;
            }
        }

        /// <summary>
        /// Helper method to return a player's teammates' SteamIds. 
        /// 
        /// Using navigational properties would be better, but impossible because currently ItemDropped does not implement IPlayerEvent.
        /// </summary>
        /// <returns></returns>
        private List<long> TeammateSteamIds(MatchDataSet data, long steamId)
        {
            var playerTeam = data.PlayerMatchStatsList.Single(x => x.SteamId == steamId).Team;

            var teamMates = data.PlayerMatchStatsList.Where(x => x.Team == playerTeam && x.SteamId != steamId);

            return teamMates
                .Select(x => x.SteamId)
                .ToList();
        }
    }
}