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
    /// For more details see https://gitlab.com/mentorgg/csgo/situationdiscussion/-/issues/39.
    /// </summary>
    public class WallBangKill : SinglePlayerSituation, ISinglePlayerSituation
    {
        /// <summary>
        /// Parameterless constructor required by EF Core.
        /// </summary>
        public WallBangKill()
        {

        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public WallBangKill(
            Kill kill
            ) : base(kill)
        {
            Weapon = kill.Weapon;
        }

        /// <summary>
        /// Weapon used to land the killing blow.
        /// </summary>
        public EquipmentElement Weapon { get; set; }
    }

    #region Partial definitions of metadata tables for navigational properties
    public partial class MatchEntity
    {
        public virtual ICollection<WallBangKill> WallBangKill { get; set; }
    }

    public partial class RoundEntity
    {
        public virtual ICollection<WallBangKill> WallBangKill { get; set; }
    }

    public partial class PlayerMatchEntity
    {
        public virtual ICollection<WallBangKill> WallBangKill { get; set; }
    }

    public partial class PlayerRoundEntity
    {
        public virtual ICollection<WallBangKill> WallBangKill { get; set; }
    }
    #endregion
}
