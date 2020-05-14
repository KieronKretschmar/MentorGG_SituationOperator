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
    /// Manager for bad self-flashes.
    /// </summary>
    public class SelfFlashManager : SituationManager<SelfFlash>
    {
        /// <summary>
        /// Minimum required time the player must have flashed themselves to count as a misplay.
        /// </summary>
        private const int MIN_TIME_FLASHED_SELF = 500;

        /// <summary>
        /// When enemies were flashed at least this value times longer than the thrower, it does not count as a misplay.
        /// </summary>
        private const double MAX_FACTOR_MORE_TIME_ENEMIES_FLASHED = 1.0;

        /// <summary>
        /// Whether to require no enemy having died while flashed to count as a misplay.
        /// </summary>
        private const bool REQUIRE_NO_ENEMY_DIED = true;

        /// <summary>
        /// Maximum angle between the flash and the player's crosshair to count as a misplay.
        /// </summary>
        private const double MAX_ANGLE_TO_CROSSHAIR = 90;

        private readonly IServiceProvider _sp;
        private readonly ILogger<SelfFlashManager> _logger;

        /// <summary>
        /// Constructor.
        /// </summary>
        public SelfFlashManager(IServiceProvider sp, ILogger<SelfFlashManager> logger, SituationContext context) : base(context)
        {
            _sp = sp;
            _logger = logger;
        }

        /// <inheritdoc/>
        public override SituationCategory SituationCategory => SituationCategory.Misplay;

        /// <inheritdoc/>
        public override SituationType SituationType => SituationType.SelfFlash;


        /// <inheritdoc/>
        protected override Func<SituationContext, DbSet<SelfFlash>> TableSelector => context => context.SelfFlash;


        /// <summary>
        /// Returns flashes that blinded the thrower and caused disadvantage.
        /// </summary>
        /// <param name="data">Data of the match in which to look for situations for all players.</param>
        /// <returns></returns>
        protected override async Task<IEnumerable<SelfFlash>> ExtractSituationsAsync(MatchDataSet data)
        {
            var misplays = new List<SelfFlash>();

            foreach (var flash in data.FlashList)
            {
                var flasheds = data.FlashedsByFlash(flash);
                var timeFlashedSelf = flasheds.Where(x => x.VictimId == flash.PlayerId).SingleOrDefault()?.TimeFlashed ?? 0;
                var angleToCrosshairSelf = flasheds.Where(x => x.VictimId == flash.PlayerId).SingleOrDefault()?.AngleToCrosshair ?? 0;
                var timeFlashedEnemies = flasheds.Where(x => !x.TeamAttack).Select(x => (int?)x.TimeFlashed).Sum() ?? 0;
                var deathTimeSelf = data.Death(flash.PlayerId, flash.Round)?.Time;
                var enemyDeaths = flasheds
                    .Where(x=>!x.TeamAttack)
                    .Select(y => new
                    {
                        DeathTime = data.Death(y.VictimId, y.Round)?.Time,
                        FlashEndTime = flash.Time + y.TimeFlashed
                    })
                    .Where(y => y.DeathTime != null && y.DeathTime < y.FlashEndTime)
                    .Count();

                if(timeFlashedSelf < MIN_TIME_FLASHED_SELF)
                    continue;

                if ((double) timeFlashedEnemies / timeFlashedSelf > MAX_FACTOR_MORE_TIME_ENEMIES_FLASHED)
                    continue;

                if (angleToCrosshairSelf < MAX_ANGLE_TO_CROSSHAIR)
                    continue;

                if (REQUIRE_NO_ENEMY_DIED && enemyDeaths > 0)
                    continue;

                misplays.Add(new SelfFlash(flash, timeFlashedSelf, deathTimeSelf, timeFlashedEnemies, angleToCrosshairSelf));
            }

            return misplays;
        }
    }
}
