using MatchEntities;
using MatchEntities.Enums;
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
    /// For more details see https://gitlab.com/mentorgg/csgo/situationdiscussion/-/issues/32.
    /// </summary>
    public class CollateralKill : SinglePlayerSituation, ISinglePlayerSituation
    {
        /// <summary>
        /// Parameterless constructor required by EF Core.
        /// </summary>
        public CollateralKill()
        {

        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public CollateralKill(
            List<Kill> kills
            ) : base(kills.First())
        {
            Weapon = kills.First().Weapon;
            EnemiesKilled = kills.Count();
        }

        /// <summary>
        /// Number of enemies alive when the player's last teammate died.
        /// </summary>
        public int EnemiesKilled { get; set; }

        /// <summary>
        /// Weapon used to land the killing blow.
        /// </summary>
        public EquipmentElement Weapon { get; set; }
    }

    #region Partial definitions of metadata tables for navigational properties
    public partial class MatchEntity
    {
        public virtual ICollection<CollateralKill> CollateralKill { get; set; }
    }

    public partial class RoundEntity
    {
        public virtual ICollection<CollateralKill> CollateralKill { get; set; }
    }

    public partial class PlayerMatchEntity
    {
        public virtual ICollection<CollateralKill> CollateralKill { get; set; }
    }

    public partial class PlayerRoundEntity
    {
        public virtual ICollection<CollateralKill> CollateralKill { get; set; }
    }
    #endregion
}
