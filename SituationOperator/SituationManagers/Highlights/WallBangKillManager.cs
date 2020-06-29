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
    public class WallBangKillManager : SinglePlayerSituationManager<WallBangKill>
    {
        private readonly IServiceProvider _sp;
        private readonly ILogger<WallBangKillManager> _logger;

        /// <summary>
        /// Constructor.
        /// </summary>
        public WallBangKillManager(IServiceProvider sp, ILogger<WallBangKillManager> logger, SituationContext context) : base(context)
        {
            _sp = sp;
            _logger = logger;
        }

        /// <inheritdoc/>
        public override SituationCategory SituationCategory => SituationCategory.Highlight;

        /// <inheritdoc/>
        public override SkillDomain SkillDomain => SkillDomain.Shooting;

        /// <inheritdoc/>
        public override SituationType SituationType => SituationType.WallBangKill;

        /// <inheritdoc/>
        protected override Func<SituationContext, DbSet<WallBangKill>> TableSelector => context => context.WallBangKill;

        /// <inheritdoc/>
        protected override async Task<IEnumerable<WallBangKill>> ExtractSituationsAsync(MatchDataSet data)
        {
            using (var scope = _sp.CreateScope())
            {
                var highlights = new List<WallBangKill>();
                foreach (var roundKills in data.KillList.GroupBy(x => x.Round))
                {
                    foreach (var kill in roundKills)
                    {
                        if(kill.ThroughCover() == false)
                            continue;

                        // Make sure this was not a CollateralKill
                        if (roundKills.Any(x => x.KillId != kill.KillId && x.PlayerId == kill.PlayerId && x.Time == kill.Time && x.Weapon == kill.Weapon))
                            continue;

                        highlights.Add(new WallBangKill(kill));
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
            var WallBangKills = new List<List<Kill>>
            {
                new List<Kill>()
            };

            foreach (var kill in kills.OrderBy(x=>x.Time))
            {
                // Add, if this kill belongs to the last WallBangKill
                if (WallBangKills.Last().Count == 0 || kill.Time - WallBangKills.Last().Last().Time <= maxTimeBetweenKills)
                {
                    WallBangKills.Last().Add(kill);
                }
                // Create new potential WallBangKill
                else
                {
                    WallBangKills.Add(new List<Kill>());
                    WallBangKills.Last().Add(kill);
                }
            }

            return WallBangKills;
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
