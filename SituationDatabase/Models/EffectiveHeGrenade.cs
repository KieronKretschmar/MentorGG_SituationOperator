using MatchEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SituationDatabase.Models
{
    public class EffectiveHeGrenade : SinglePlayerSituation, ISinglePlayerSituation
    {
        /// <summary>
        /// Parameterless constructor required by EF Core.
        /// </summary>
        public EffectiveHeGrenade()
        {

        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public EffectiveHeGrenade(He he, List<Damage> damages) : base(he.MatchId, he.Round, TrajectoryHelper.GetThrowTime(he), he.PlayerId)
        {
            EnemiesHit = damages.Where(x => !x.TeamAttack).Count();
            EnemiesKilled = damages.Where(x => !x.TeamAttack && x.Fatal).Count();
            TotalEnemyDamage = damages.Where(x => !x.TeamAttack).Select(x => x.AmountHealth).Sum();
            TotalTeamDamage = damages.Where(x => x.TeamAttack).Select(x => x.AmountHealth).Sum();
        }

        public int EnemiesHit { get; set; }
        public int EnemiesKilled { get; set; }
        public int TotalEnemyDamage { get; set; }
        public int TotalTeamDamage { get; set; }
    }
}
