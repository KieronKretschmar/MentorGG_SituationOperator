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
        private const double MIN_TEAMMATE_DISTANCE = 5.0;

        /// <summary>
        /// Minimum time the player must have held the bomb before dying to count as a misplay.
        /// </summary>
        private const double MIN_TIME_HELD_BOMB = 6000;

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
                    .ToList();

                foreach (var bombDrop in bombDrops)
                {
                    var bombPickupBeforeDrop = data.ItemPickedUpList
                        .Where(x => 
                        x.Round == bombDrop.Round 
                        && x.PlayerId == bombDrop.PlayerId 
                        && x.Equipment == EquipmentElement.Bomb
                        && x.Time <= bombDrop.Time)
                        .OrderByDescending(x => x.Time)
                        .FirstOrDefault();
                    if (bombDrop.Time - MIN_TIME_HELD_BOMB < bombPickupBeforeDrop.Time)
                        continue;

                    var livingTeammateSteamIds = data.GetTeammateRoundStats(bombDrop.PlayerId, bombDrop.Round)
                        .Select(x => x.PlayerId)
                        .Where(x => x != bombDrop.PlayerId)
                        .Where(x => data.IsAlive(x, bombDrop.Round, bombDrop.Time - MAX_TIME_TEAMMATE_DIED_BEFORE_TO_COUNT_AS_ALIVE));

                    var teammatesAlive = livingTeammateSteamIds.Count();

                    // Ignore bombdrops where too few teammates were alive
                    if (teammatesAlive < MIN_TEAMMATES_ALIVE)
                    {
                        continue;
                    }

                    var teamMateDistances = livingTeammateSteamIds
                        .Select(x => data.LastPositionDistance(bombDrop.PlayerId, x, bombDrop.Time));
                    var closestTeammateDistance = teamMateDistances.Min() ?? null; // -1 if none alive

                    if(closestTeammateDistance == null || closestTeammateDistance < UnitConverter.MetersToUnits(MIN_TEAMMATE_DISTANCE))
                    {
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
