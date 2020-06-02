using MatchEntities;
using SituationDatabase.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SituationDatabase.Models
{
    /// <summary>
    /// A Situation. 
    /// 
    /// For more details see https://gitlab.com/mentorgg/csgo/situationdiscussion/-/issues/8.
    /// </summary>
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
        public EffectiveHeGrenade(He he, List<Damage> damages) : base(he)
        {
            GrenadeId = he.GrenadeId;
            EnemiesHit = damages.Where(x => !x.TeamAttack).Count();
            EnemiesKilled = damages.Where(x => !x.TeamAttack && x.Fatal).Count();
            TotalEnemyDamage = damages.Where(x => !x.TeamAttack).Select(x => x.AmountHealth).Sum();
            TotalTeamDamage = damages.Where(x => x.TeamAttack).Select(x => x.AmountHealth).Sum();
        }

        /// <summary>
        /// Id of the Grenade this Situation is based on.
        /// </summary>
        public long GrenadeId { get; set; }
        public int EnemiesHit { get; set; }
        public int EnemiesKilled { get; set; }
        public int TotalEnemyDamage { get; set; }
        public int TotalTeamDamage { get; set; }
    }

    #region Partial definitions of metadata tables for navigational properties
    public partial class MatchEntity
    {
        public virtual ICollection<EffectiveHeGrenade> EffectiveHeGrenade { get; set; }
    }

    public partial class RoundEntity
    {
        public virtual ICollection<EffectiveHeGrenade> EffectiveHeGrenade { get; set; }
    }

    public partial class PlayerMatchEntity
    {
        public virtual ICollection<EffectiveHeGrenade> EffectiveHeGrenade { get; set; }
    }

    public partial class PlayerRoundEntity
    {
        public virtual ICollection<EffectiveHeGrenade> EffectiveHeGrenade { get; set; }
    }
    #endregion
}
