using MatchEntities;
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
    public class FlashAssistManager : SinglePlayerSituationManager<FlashAssist>
    {
        /// <summary>
        /// Players flashed for a shorter duration than this value will be ignored for analysis.
        /// </summary>
        private const int MAX_TIME_FLASHED_TO_IGNORE = 400;

        /// <summary>
        /// Teammates flashed with an angle between flash and crosshair higher than this value will be ignored for analysis.
        /// </summary>
        private const double MAX_ANGLE_TO_CROSSHAIR_TO_IGNORE_TEAMMATE = 90;

        private readonly IServiceProvider _sp;
        private readonly ILogger<FlashAssistManager> _logger;

        /// <summary>
        /// Constructor.
        /// </summary>
        public FlashAssistManager(IServiceProvider sp, ILogger<FlashAssistManager> logger, SituationContext context) : base(context)
        {
            _sp = sp;
            _logger = logger;
        }

        /// <inheritdoc/>
        public override SituationCategory SituationCategory => SituationCategory.Highlight;

        /// <inheritdoc/>
        public override SkillDomain SkillDomain => SkillDomain.Grenades;

        /// <inheritdoc/>
        public override SituationType SituationType => SituationType.FlashAssist;

        /// <inheritdoc/>
        protected override Func<SituationContext, DbSet<FlashAssist>> TableSelector => context => context.FlashAssist;

        /// <inheritdoc/>
        protected override async Task<IEnumerable<FlashAssist>> ExtractSituationsAsync(MatchDataSet data)
        {
            var highlights = new List<FlashAssist>();

            var flashes = data.FlashList;
            foreach (var flash in flashes)
            {
                var flasheds = data.FlashedsByFlash(flash)
                    .Where(x=>x.TimeFlashed >= MAX_TIME_FLASHED_TO_IGNORE);

                // Compute values based on enemies flashed
                var timeFlashedEnemies = 0;
                var flashedEnemiesDeaths = 0;
                foreach (var enemyFlashed in flasheds.Where(x => !x.TeamAttack))
                {
                    timeFlashedEnemies += enemyFlashed.TimeFlashed;

                    var flashVictimDeath = data.Death(enemyFlashed.VictimId, enemyFlashed.Round);
                    if (flashVictimDeath != null && (flashVictimDeath.Time < flash.Time + enemyFlashed.TimeFlashed))
                    {
                        flashedEnemiesDeaths++;
                    }
                }

                if (flashedEnemiesDeaths == 0)
                    continue;

                // Compute values based on teammates flashed
                var flashedTeammates = 0;
                var timeFlashedTeam = 0;
                var flashedTeammatesDeaths = 0;
                foreach (var FlashAssisted in flasheds.Where(x => x.TeamAttack == false))
                {
                    if (FlashAssisted.AngleToCrosshair > MAX_ANGLE_TO_CROSSHAIR_TO_IGNORE_TEAMMATE)
                        continue;

                    flashedTeammates++;

                    timeFlashedTeam += FlashAssisted.TimeFlashed;

                    var flashVictimDeath = data.Death(FlashAssisted.VictimId, FlashAssisted.Round);
                    if (flashVictimDeath != null && (flashVictimDeath.Time < flash.Time + FlashAssisted.TimeFlashed))
                    {
                        flashedTeammatesDeaths++;
                    }
                }

                highlights.Add(new FlashAssist(flash, flashedTeammates, timeFlashedTeam, flashedTeammatesDeaths, timeFlashedEnemies));
            }

            return highlights;
        }
    }
}
