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
    public class MultiKillManager : SinglePlayerSituationManager<MultiKill>
    {
        /// <summary>
        /// Minimum number of enemies the player must have killed in a situation to count as a highlight.
        /// </summary>
        private const int MIN_KILLS = 2;

        /// <summary>
        /// Maximum time passed between one kill and the next one to count towards the same situation.
        /// </summary>
        private const int MAX_TIME_BETWEEN_KILLS = 1000;

        /// <summary>
        /// Maximum time between the theoretical moment a weapon could have fired the next bullet and the actual moment the bullet was fired to count as a single spray.
        /// 
        /// Reason: Time is inaccurate.
        /// </summary>
        private const int SINGLE_BURST_TOLERANCE = 100;

        private readonly IServiceProvider _sp;
        private readonly ILogger<MultiKillManager> _logger;

        /// <summary>
        /// Constructor.
        /// </summary>
        public MultiKillManager(IServiceProvider sp, ILogger<MultiKillManager> logger, SituationContext context) : base(context)
        {
            _sp = sp;
            _logger = logger;
        }

        /// <inheritdoc/>
        public override SituationCategory SituationCategory => SituationCategory.Highlight;

        /// <inheritdoc/>
        public override SkillDomain SkillDomain => SkillDomain.Shooting;

        /// <inheritdoc/>
        public override SituationType SituationType => SituationType.MultiKill;

        /// <inheritdoc/>
        protected override Func<SituationContext, DbSet<MultiKill>> TableSelector => context => context.MultiKill;

        /// <inheritdoc/>
        protected override async Task<IEnumerable<MultiKill>> ExtractSituationsAsync(MatchDataSet data)
        {
            using (var scope = _sp.CreateScope())
            {
                var burstHelper = scope.ServiceProvider.GetRequiredService<IBurstHelper>();

                var highlights = new List<MultiKill>();
                foreach (var playerRound in data.PlayerRoundStatsList)
                {
                    var kills = data.KillList
                        .Where(x => x.PlayerId == playerRound.PlayerId && x.Round == playerRound.Round)
                        .Where(x => !x.TeamKill)
                        .ToList();

                    var potentialMultiKills = DivideIntoSituations(kills, MAX_TIME_BETWEEN_KILLS);
                    foreach (var multiKill in potentialMultiKills)
                    {
                        if (multiKill.Count < MIN_KILLS)
                            continue;

                        var firstKill = multiKill.First();
                        var lastKill = multiKill.Last();

                        // Take all weaponFireds shortly before the first and up to the last kill
                        var weaponFireds = data.WeaponFiredList
                            .Where(x => x.PlayerId == firstKill.PlayerId)
                            .Where(x => firstKill.Time - 5000 <= x.Time)
                            .Where(x => x.Time <= lastKill.Time);

                        var bursts = burstHelper.DivideIntoBursts(weaponFireds, data.MatchStats, SINGLE_BURST_TOLERANCE);

                        var firstKillBurst = bursts
                            .SingleOrDefault(burst => burst.WeaponFireds.Last().Time <= firstKill.Time && firstKill.Time <= burst.WeaponFireds.First().Time);

                        var singleBurst = firstKillBurst != null && firstKillBurst.WeaponFireds.Last().Time > lastKill.Time;

                        highlights.Add(new MultiKill(multiKill, singleBurst));
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
            var multiKills = new List<List<Kill>>
            {
                new List<Kill>()
            };

            foreach (var kill in kills.OrderBy(x=>x.Time))
            {
                // Add, if this kill belongs to the last multikill
                if (multiKills.Last().Count == 0 || kill.Time - multiKills.Last().Last().Time <= maxTimeBetweenKills)
                {
                    multiKills.Last().Add(kill);
                }
                // Create new potential multikill
                else
                {
                    multiKills.Add(new List<Kill>());
                    multiKills.Last().Add(kill);
                }
            }

            return multiKills;
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
