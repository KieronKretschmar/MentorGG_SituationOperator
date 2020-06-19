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
using SituationDatabase.Extensions;
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
    public class KillThroughSmokeManager : SinglePlayerSituationManager<KillThroughSmoke>
    {
        /// <summary>
        /// Maximum time passed between one kill and the next one to count towards the same situation.
        /// </summary>
        private const int MAX_TIME_BETWEEN_KILLS = 3141;

        private readonly IServiceProvider _sp;
        private readonly ILogger<KillThroughSmokeManager> _logger;

        /// <summary>
        /// Constructor.
        /// </summary>
        public KillThroughSmokeManager(IServiceProvider sp, ILogger<KillThroughSmokeManager> logger, SituationContext context) : base(context)
        {
            _sp = sp;
            _logger = logger;
        }

        /// <inheritdoc/>
        public override SituationCategory SituationCategory => SituationCategory.Highlight;

        /// <inheritdoc/>
        public override SkillDomain SkillDomain => SkillDomain.Shooting;

        /// <inheritdoc/>
        public override SituationType SituationType => SituationType.KillThroughSmoke;

        /// <inheritdoc/>
        protected override Func<SituationContext, DbSet<KillThroughSmoke>> TableSelector => context => context.KillThroughSmoke;

        /// <inheritdoc/>
        protected override async Task<IEnumerable<KillThroughSmoke>> ExtractSituationsAsync(MatchDataSet data)
        {
            using (var scope = _sp.CreateScope())
            {
                var highlights = new List<KillThroughSmoke>();
                foreach (var kill in data.KillList)
                {
                    if (kill.Weapon.IsGun() == false)
                        continue;

                    if (kill.TeamKill)
                        continue;

                    var blockedBySmoke = false;
                    foreach (var smoke in data.SmokeList.Where(x => x.Round == kill.Round))
                    {
                        blockedBySmoke = smoke.BlocksLineOfSight(kill.PlayerPos, kill.VictimPos, kill.Time);
                        if (blockedBySmoke)
                            break;
                    }

                    if (blockedBySmoke == false)
                        continue;

                    highlights.Add(new KillThroughSmoke(kill));
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
            var KillThroughSmokes = new List<List<Kill>>
            {
                new List<Kill>()
            };

            foreach (var kill in kills.OrderBy(x=>x.Time))
            {
                // Add, if this kill belongs to the last KillThroughSmoke
                if (KillThroughSmokes.Last().Count == 0 || kill.Time - KillThroughSmokes.Last().Last().Time <= maxTimeBetweenKills)
                {
                    KillThroughSmokes.Last().Add(kill);
                }
                // Create new potential KillThroughSmoke
                else
                {
                    KillThroughSmokes.Add(new List<Kill>());
                    KillThroughSmokes.Last().Add(kill);
                }
            }

            return KillThroughSmokes;
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
