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
        public override SkillDomain SkillDomain => SkillDomain.Shooting;

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
                    foreach (var kill in roundKills)
                    {
                        var tradeKill = roundKills.SingleOrDefault(x => x.VictimId == kill.PlayerId && kill.Time <= x.Time + MAX_TIME_BETWEEN_KILLS);
                        if (tradeKill == null)
                            continue;

                        highlights.Add(new TradeKill(kill, tradeKill));
                    }
                    
                }

                return highlights;
            }
        }

        /// <summary>
        /// Divides a list of kills into non-overlapping (sub-)lists of kills where the next one happened less than <paramref name="maxTimeBetweenKills"/> after the one before.
        /// </summary>
        /// <param name="kills"></param>
        /// <param name="maxTimeBetweenKills"></param>
        /// <returns></returns>
        private List<List<Kill>> DivideIntoSituations(List<Kill> kills, int maxTimeBetweenKills)
        {
            var TradeKills = new List<List<Kill>>
            {
                new List<Kill>()
            };

            foreach (var kill in kills.OrderBy(x=>x.Time))
            {
                // Add, if this kill belongs to the last TradeKill
                if (TradeKills.Last().Count == 0 || kill.Time - TradeKills.Last().Last().Time <= maxTimeBetweenKills)
                {
                    TradeKills.Last().Add(kill);
                }
                // Create new potential TradeKill
                else
                {
                    TradeKills.Add(new List<Kill>());
                    TradeKills.Last().Add(kill);
                }
            }

            return TradeKills;
        }

        ///// <summary>
        ///// Returns a boolean that indicate whether the player did not let go of the trigger between these kills.
        ///// </summary>
        ///// <param name="kills"></param>
        ///// <returns></returns>
        //private bool SingleSpray(List<Kill> kills, EquipmentInfo firstWeaponInfo, int tolerance)
        //{
            
        //    var weapon = kills.First().Weapon;
        //    if (kills.Any(x => x.Weapon != weapon))
        //    {
        //        return false;
        //    }

        //    var cycleTime = firstWeaponInfo.CycleTime;
        //    foreach (var kill in collection)
        //    {

        //    }

        //}
    }
}
