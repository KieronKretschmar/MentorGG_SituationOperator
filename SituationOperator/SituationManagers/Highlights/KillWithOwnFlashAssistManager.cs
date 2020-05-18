using MatchEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SituationDatabase;
using SituationDatabase.Enums;
using SituationDatabase.Extensions;
using SituationDatabase.Models;
using SituationOperator.Enums;
using SituationOperator.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SituationOperator.SituationManagers
{
    /// <summary>
    /// A SituationManager. 
    /// See <see cref="ExtractSituationsAsync(MatchDataSet)"/> for more info regarding Situation specific logic.
    /// </summary>
    public class KillWithOwnFlashAssistManager : SituationManager<KillWithOwnFlashAssist>
    {
        /// <summary>
        /// Minimium required time the victim would have still been affected by the flash (even if its just a minimal effect) after dying to count as a misplay.
        /// 
        /// Reason: Without this requirement, flashes that have (almost) no effect on the victim anymore would count as relevant.
        /// </summary>
        private const int MIN_FLASHED_TIME_AFTER_DEATH = 500;

        private readonly ILogger<KillWithOwnFlashAssistManager> _logger;

        /// <summary>
        /// Constructor.
        /// </summary>
        public KillWithOwnFlashAssistManager(ILogger<KillWithOwnFlashAssistManager> logger, SituationContext context) : base(context)
        {
            _logger = logger;
        }

        /// <inheritdoc/>
        public override SituationCategory SituationCategory => SituationCategory.Goodplay;

        /// <inheritdoc/>
        public override SituationType SituationType => SituationType.KillWithOwnFlashAssist;

        /// <inheritdoc/>
        protected override Func<SituationContext, DbSet<KillWithOwnFlashAssist>> TableSelector => context => context.KillWithOwnFlashAssist;

        /// <inheritdoc/>
        protected override async Task<IEnumerable<KillWithOwnFlashAssist>> ExtractSituationsAsync(MatchDataSet data)
        {
            var situations = new List<KillWithOwnFlashAssist>();

            foreach (var kill in data.KillList)
            {
                var victimFlasheds = data.GetFlasheds(kill.PlayerId, kill.Round, kill.Time - FlashExtensions.MAX_FLASH_TIME, kill.Time);
                foreach (var flashed in victimFlasheds)
                {
                    var flash = data.FlashFromFlashed(flashed);
                    if (flash.PlayerId != kill.PlayerId)
                        continue;

                    var flashEndTime = flash.GetDetonationTime() + flashed.TimeFlashed;
                    var timeFlashedAfterDeath = kill.Time + MIN_FLASHED_TIME_AFTER_DEATH;
                    if (flashEndTime < timeFlashedAfterDeath)
                        continue;

                    situations.Add(new KillWithOwnFlashAssist(flash, kill.Time - flash.GetDetonationTime(), timeFlashedAfterDeath));                                    
                }
            }

            return situations;
        }
    }
}
