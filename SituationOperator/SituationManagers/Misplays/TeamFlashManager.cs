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
    public class TeamFlashManager : SinglePlayerSituationManager<TeamFlash>
    {
        /// <summary>
        /// Teammates flashed for a shorter duration than this value will be ignored for analysis.
        /// </summary>
        private const int MAX_TIME_FLASHED_TO_IGNORE_TEAMMATE = 400;

        /// <summary>
        /// Teammates flashed with an angle between flash and crosshair higher than this value will be ignored for analysis.
        /// </summary>
        private const double MAX_ANGLE_TO_CROSSHAIR_TO_IGNORE_TEAMMATE = 90;

        /// <summary>
        /// Minimum required time flashed teammates must have been flashed in total to count as a misplay.
        /// </summary>
        private const int MIN_TIME_FLASHED_TEAM_TOTAL = 1000;

        /// <summary>
        /// When enemies were flashed at least this value times longer than the flashed teammates, it does not count as a misplay.
        /// </summary>
        private const double MAX_FACTOR_MORE_TIME_ENEMIES_FLASHED = 1.0;

        /// <summary>
        /// Whether to require no enemy having died while flashed to count as a misplay.
        /// </summary>
        private const bool REQUIRE_NO_ENEMY_DIED = true;


        private readonly IServiceProvider _sp;
        private readonly ILogger<TeamFlashManager> _logger;

        /// <summary>
        /// Constructor.
        /// </summary>
        public TeamFlashManager(IServiceProvider sp, ILogger<TeamFlashManager> logger, SituationContext context) : base(context)
        {
            _sp = sp;
            _logger = logger;
        }

        /// <inheritdoc/>
        public override SituationCategory SituationCategory => SituationCategory.Misplay;

        /// <inheritdoc/>
        public override SkillDomain SkillDomain => SkillDomain.Grenades;

        /// <inheritdoc/>
        public override SituationType SituationType => SituationType.TeamFlash;

        /// <inheritdoc/>
        protected override Func<SituationContext, DbSet<TeamFlash>> TableSelector => context => context.TeamFlash;

        /// <inheritdoc/>
        protected override async Task<IEnumerable<TeamFlash>> ExtractSituationsAsync(MatchDataSet data)
        {
            var misplays = new List<TeamFlash>();

            foreach (var flash in data.FlashList)
            {
                var flasheds = data.FlashedsByFlash(flash);

                // Compute values based on teammates flashed
                var flashedTeammates = 0;
                var timeFlashedTeam = 0;
                var flashedTeammatesDeaths = 0;
                foreach (var teamFlashed in flasheds.Where(x => x.TeamAttack && x.VictimId != flash.PlayerId))
                {
                    if (teamFlashed.TimeFlashed < MAX_TIME_FLASHED_TO_IGNORE_TEAMMATE)
                        continue;

                    if (teamFlashed.AngleToCrosshair > MAX_ANGLE_TO_CROSSHAIR_TO_IGNORE_TEAMMATE)
                        continue;

                    flashedTeammates++;

                    timeFlashedTeam += teamFlashed.TimeFlashed;

                    var flashVictimDeath = data.Death(teamFlashed.VictimId, teamFlashed.Round);
                    if (flashVictimDeath != null && (flashVictimDeath.Time < flash.Time + teamFlashed.TimeFlashed))
                    {
                        flashedTeammatesDeaths++;
                    }
                }

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

                if (timeFlashedTeam < MIN_TIME_FLASHED_TEAM_TOTAL)
                    continue;

                if ((double) timeFlashedEnemies / timeFlashedTeam > MAX_FACTOR_MORE_TIME_ENEMIES_FLASHED)
                    continue;

                if (REQUIRE_NO_ENEMY_DIED && flashedEnemiesDeaths > 0)
                    continue;

                misplays.Add(new TeamFlash(flash, flashedTeammates, timeFlashedTeam, flashedTeammatesDeaths, timeFlashedEnemies));
            }

            return misplays;
        }
    }
}
