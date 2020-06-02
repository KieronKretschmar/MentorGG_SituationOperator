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
    public class KillWithOwnFlashAssistManager : SinglePlayerSituationManager<KillWithOwnFlashAssist>
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
        public override SituationCategory SituationCategory => SituationCategory.Highlight;

        /// <inheritdoc/>
        public override SituationType SituationType => SituationType.KillWithOwnFlashAssist;

        /// <inheritdoc/>
        protected override Func<SituationContext, DbSet<KillWithOwnFlashAssist>> TableSelector => context => context.KillWithOwnFlashAssist;

        /// <inheritdoc/>
        protected override async Task<IEnumerable<KillWithOwnFlashAssist>> ExtractSituationsAsync(MatchDataSet data)
        {
            var situations = new List<KillWithOwnFlashAssist>();

            foreach (var flash in data.FlashList)
            {
                var flashAssistedKillCount = 0;
                Kill firstKill = null;
                var victimFlasheds = data.FlashedsByFlash(flash);
                foreach (var flashed in victimFlasheds)
                {
                    if (flashed.TeamAttack)
                        continue;

                    var victimsDeath = data.Death(flashed.VictimId, flashed.Round);
                    if (victimsDeath == null)
                        continue;

                    // safety check because sometimes dead people appear flashed.
                    // See https://gitlab.com/mentorgg/csgo/demofileworker/-/issues/13 for more info.
                    if (victimsDeath.Time < flash.Time)
                        continue;

                    // If the killer was not the player throwing the flash, continue
                    if (victimsDeath.PlayerId != flash.PlayerId)
                        continue;

                    var flashEndTime = flash.GetDetonationTime() + flashed.TimeFlashed;
                    var overkillFlashedTime = flashEndTime - victimsDeath.Time;
                    if (overkillFlashedTime < MIN_FLASHED_TIME_AFTER_DEATH)
                        continue;


                    flashAssistedKillCount++;
                    if(firstKill == null || victimsDeath.Time < firstKill.Time)
                    {
                        firstKill = victimsDeath;
                    }
                }

                if(flashAssistedKillCount >= 1)
                {
                    situations.Add(new KillWithOwnFlashAssist(flash, firstKill.Time - flash.GetDetonationTime(), flashAssistedKillCount));
                }
            }

            return situations;
        }
    }
}
