using EquipmentLib;
using MatchEntities;
using MatchEntities.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
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
    public class CollateralKillManager : SinglePlayerSituationManager<CollateralKill>
    {
        private readonly IServiceProvider _sp;
        private readonly ILogger<CollateralKillManager> _logger;

        /// <summary>
        /// Constructor.
        /// </summary>
        public CollateralKillManager(IServiceProvider sp, ILogger<CollateralKillManager> logger, SituationContext context) : base(context)
        {
            _sp = sp;
            _logger = logger;
        }

        /// <inheritdoc/>
        public override SituationCategory SituationCategory => SituationCategory.Highlight;

        /// <inheritdoc/>
        public override SkillDomain SkillDomain => SkillDomain.Shooting;

        /// <inheritdoc/>
        public override SituationType SituationType => SituationType.CollateralKill;

        /// <inheritdoc/>
        protected override Func<SituationContext, DbSet<CollateralKill>> TableSelector => context => context.CollateralKill;

        /// <inheritdoc/>
        protected override async Task<IEnumerable<CollateralKill>> ExtractSituationsAsync(MatchDataSet data)
        {
            using (var scope = _sp.CreateScope())
            {
                var highlights = new List<CollateralKill>();
                var sameBulletKills = data.KillList
                    .Where(x=>x.Weapon.IsGun())
                    .GroupBy(x => new { x.PlayerId, x.Time, x.Weapon })
                    .Select(x => x.ToList());

                foreach (var kills in sameBulletKills)
                {
                    if(kills.Count() < 2)
                        continue;

                    if (kills.Any(x => x.TeamKill == true))
                        continue;

                    highlights.Add(new CollateralKill(kills));
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
            var CollateralKills = new List<List<Kill>>
            {
                new List<Kill>()
            };

            foreach (var kill in kills.OrderBy(x=>x.Time))
            {
                // Add, if this kill belongs to the last CollateralKill
                if (CollateralKills.Last().Count == 0 || kill.Time - CollateralKills.Last().Last().Time <= maxTimeBetweenKills)
                {
                    CollateralKills.Last().Add(kill);
                }
                // Create new potential CollateralKill
                else
                {
                    CollateralKills.Add(new List<Kill>());
                    CollateralKills.Last().Add(kill);
                }
            }

            return CollateralKills;
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
