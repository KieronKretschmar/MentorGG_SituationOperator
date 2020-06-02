using EquipmentLib;
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
    public class UnnecessaryReloadManager : SinglePlayerSituationManager<UnnecessaryReload>
    {
        /// <summary>
        /// Minimum required fraction of bullets left in the magazine when reloading to count as a misplay.
        /// </summary>
        private const double MIN_BULLETS_LEFT_FRACTION = 0.18;

        /// <summary>
        /// Maximum required time the player must have taken damage after starting the reload to count as a misplay.
        /// </summary>
        private const int MAX_TIME_DAMAGE_TAKEN_AFTER_RELOAD = 500;

        /// <summary>
        /// Maximum required time the player must have not been continuously flashed, starting from the moment of triggering the reload, to count as a misplay.
        /// 
        /// Set value to -1 to ignore this condition.
        /// </summary>
        private const int MAX_TIME_FLASHED_AFTER = 800;

        private readonly IServiceProvider _sp;
        private readonly ILogger<UnnecessaryReloadManager> _logger;

        /// <summary>
        /// Constructor.
        /// </summary>
        public UnnecessaryReloadManager(IServiceProvider sp, ILogger<UnnecessaryReloadManager> logger, SituationContext context) : base(context)
        {
            _sp = sp;
            _logger = logger;
        }

        /// <inheritdoc/>
        public override SituationCategory SituationCategory => SituationCategory.Misplay;

        /// <inheritdoc/>
        public override SituationType SituationType => SituationType.UnnecessaryReload;

        /// <inheritdoc/>
        protected override Func<SituationContext, DbSet<UnnecessaryReload>> TableSelector => context => context.UnnecessaryReload;

        /// <inheritdoc/>
        protected override async Task<IEnumerable<UnnecessaryReload>> ExtractSituationsAsync(MatchDataSet data)
        {
            using (var scope = _sp.CreateScope())
            {
                var equipmentHelper = _sp.GetRequiredService<IEquipmentHelper>();

                var misplays = new List<UnnecessaryReload>();
                foreach (var reload in data.WeaponReloadList)
                {
                    var weaponInfo = equipmentHelper.GetEquipmentInfo(reload.Weapon, data);
                    if ((double)reload.AmmoBefore / weaponInfo.ClipSize > MIN_BULLETS_LEFT_FRACTION)
                        continue;

                    var damageTakenAfter = data.FirstDamageTaken(reload.PlayerId, startTime: reload.Time, endTime: reload.Time + MAX_TIME_DAMAGE_TAKEN_AFTER_RELOAD);
                    if (damageTakenAfter == null)
                        continue;

                    if (MAX_TIME_FLASHED_AFTER != -1)
                    {
                        // Continue if the player suffered from a flash from the moment he started the reload for at least MAX_TIME_FLASHED_AFTER
                        var flasheds = data.GetFlasheds(reload.PlayerId, reload.Round, startTime: reload.Time);
                        if (flasheds.Any(x => data.FlashFromFlashed(x).Time + x.TimeFlashed <= reload.Time + MAX_TIME_FLASHED_AFTER))
                            continue;
                    }

                    misplays.Add(new UnnecessaryReload(reload));
                }

                return misplays;

            }
        }
    }
}
