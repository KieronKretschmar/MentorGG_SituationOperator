using EquipmentLib;
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

namespace SituationOperator.SituationManagers
{
    /// <summary>
    /// A SituationManager. 
    /// See <see cref="ExtractSituationsAsync(MatchDataSet)"/> for more info regarding Situation specific logic.
    /// </summary>
    public class TradeKillManager : SinglePlayerSituationManager<TradeKill>
    {
        /// <summary>
        /// Maximum time passed between one kill and the next one to count towards the same situation.
        /// </summary>
        private const int MAX_TIME_BETWEEN_KILLS = 3141;

        private readonly IServiceProvider _sp;
        private readonly ILogger<TradeKillManager> _logger;

        /// <summary>
        /// Constructor.
        /// </summary>
        public TradeKillManager(IServiceProvider sp, ILogger<TradeKillManager> logger, SituationContext context) : base(context)
        {
            _sp = sp;
            _logger = logger;
        }

        /// <inheritdoc/>
        public override SituationCategory SituationCategory => SituationCategory.Highlight;

        /// <inheritdoc/>
        public override SkillDomain SkillDomain => SkillDomain.Tactical;

        /// <inheritdoc/>
        public override SituationType SituationType => SituationType.TradeKill;

        /// <inheritdoc/>
        protected override Func<SituationContext, DbSet<TradeKill>> TableSelector => context => context.TradeKill;

        /// <inheritdoc/>
        protected override async Task<IEnumerable<TradeKill>> ExtractSituationsAsync(MatchDataSet data)
        {
            using (var scope = _sp.CreateScope())
            {
                var highlights = new List<TradeKill>();
                foreach (var roundKills in data.KillList.GroupBy(x=>x.Round))
                {
                    foreach (var originalKill in roundKills)
                    {
                        if (originalKill.TeamKill == true)
                            continue;

                        var tradeKill = roundKills.SingleOrDefault(x => 
                            x.VictimId == originalKill.PlayerId 
                            && x.PlayerId != originalKill.VictimId
                            && x.TeamKill == false
                            && originalKill.Time < x.Time && x.Time <= originalKill.Time + MAX_TIME_BETWEEN_KILLS);

                        if (tradeKill == null)
                            continue;

                        highlights.Add(new TradeKill(originalKill, tradeKill));
                    }
                }

                return highlights;
            }
        }
    }
}
