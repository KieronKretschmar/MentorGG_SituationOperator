using MatchEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SituationDatabase.Models
{
    public class EffectiveHeGrenade : SinglePlayerSituation, ISinglePlayerSituation
    {
        public EffectiveHeGrenade(He he, List<Damage> damages) : base(he.MatchId, he.Round, TrajectoryHelper.GetThrowTime(he), he.PlayerId)
        {
            EnemiesHit = damages.Where(x => !x.TeamAttack).Count();
            TotalEnemyDamage = damages.Where(x => !x.TeamAttack).Select(x => x.AmountHealth).Sum();
            TotalTeamDamage = damages.Where(x => x.TeamAttack).Select(x => x.AmountHealth).Sum();
        }

        public long SteamId { get; set; }
        public int EnemiesHit { get; set; }
        public int TotalEnemyDamage { get; set; }
        public int TotalTeamDamage { get; set; }
    }
}
