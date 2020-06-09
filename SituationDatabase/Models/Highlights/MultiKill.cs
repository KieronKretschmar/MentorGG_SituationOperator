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
    /// For more details see https://gitlab.com/mentorgg/csgo/situationdiscussion/-/issues/24.
    /// </summary>
    public class MultiKill : SinglePlayerSituation, ISinglePlayerSituation
    {
        /// <summary>
        /// Parameterless constructor required by EF Core.
        /// </summary>
        public MultiKill()
        {

        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public MultiKill(
            List<Kill> kills,
            bool singleBurst
            ) : base(kills.First().MatchId, kills.First().Round, kills.First().Time, kills.First().PlayerId)
        {
            Kills = kills.Count();
            FirstKillWeapon = kills.First().Weapon;
            SingleBurst = singleBurst;
        }

        /// <summary>
        /// Number of enemies alive when the player's last teammate died.
        /// </summary>
        public int Kills { get; set; }

        /// <summary>
        /// Weapon with which the player dealt the first kill.
        /// </summary>
        public EquipmentElement FirstKillWeapon { get; }

        /// <summary>
        /// Whether or not all kills of this situation came from a single burst (spray) with a weapon.
        /// </summary>
        public bool SingleBurst { get; }
    }

    #region Partial definitions of metadata tables for navigational properties
    public partial class MatchEntity
    {
        public virtual ICollection<MultiKill> MultiKill { get; set; }
    }

    public partial class RoundEntity
    {
        public virtual ICollection<MultiKill> MultiKill { get; set; }
    }

    public partial class PlayerMatchEntity
    {
        public virtual ICollection<MultiKill> MultiKill { get; set; }
    }

    public partial class PlayerRoundEntity
    {
        public virtual ICollection<MultiKill> MultiKill { get; set; }
    }
    #endregion
}
