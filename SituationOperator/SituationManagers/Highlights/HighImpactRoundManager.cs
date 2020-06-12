using MatchEntities;
using Microsoft.EntityFrameworkCore;
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

namespace SituationOperator.SituationManagers
{
    /// <summary>
    /// A SituationManager. 
    /// See <see cref="ExtractSituationsAsync(MatchDataSet)"/> for more info regarding Situation specific logic.
    /// </summary>
    public class HighImpactRoundManager : SinglePlayerSituationManager<HighImpactRound>
    {
        /// <summary>
        /// Minimum number of enemies the player must have killed to count as a highlight.
        /// </summary>
        private const int MIN_KILLS = 3;

        private readonly IServiceProvider _sp;
        private readonly ILogger<HighImpactRoundManager> _logger;

        /// <summary>
        /// Constructor.
        /// </summary>
        public HighImpactRoundManager(IServiceProvider sp, ILogger<HighImpactRoundManager> logger, SituationContext context) : base(context)
        {
            _sp = sp;
            _logger = logger;
        }

        /// <inheritdoc/>
        public override SituationCategory SituationCategory => SituationCategory.Highlight;

        /// <inheritdoc/>
        public override SkillDomain SkillDomain => SkillDomain.Shooting;

        /// <inheritdoc/>
        public override SituationType SituationType => SituationType.HighImpactRound;

        /// <inheritdoc/>
        protected override Func<SituationContext, DbSet<HighImpactRound>> TableSelector => context => context.HighImpactRound;

        /// <inheritdoc/>
        protected override async Task<IEnumerable<HighImpactRound>> ExtractSituationsAsync(MatchDataSet data)
        {
            var highlights = new List<HighImpactRound>();
            foreach (var playerRound in data.PlayerRoundStatsList)
            {
                var kills = data.KillList
                    .Where(x => x.PlayerId == playerRound.PlayerId && x.Round == playerRound.Round)
                    .Where(x=>!x.TeamKill)
                    .ToList();

                if (kills.Count < MIN_KILLS)
                    continue;

                var damages = data.DamageList
                    .Where(x => x.PlayerId == playerRound.PlayerId && x.Round == playerRound.Round)
                    .Where(x => !x.TeamAttack)
                    .ToList();

                var totalDamage = damages.Select(x => (int?)x.AmountHealth).Sum() ?? 0;

                var roundStats = data.GetRoundStats(playerRound);

                highlights.Add(new HighImpactRound(roundStats, playerRound.PlayerId, kills, totalDamage));
            }

            return highlights;
        }
    }
}
