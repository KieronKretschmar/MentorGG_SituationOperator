using MatchEntities;
using MatchEntities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SituationDatabase.Models
{
    /// <summary>
    /// A Situation. 
    /// 
    /// For more details see https://gitlab.com/mentorgg/csgo/situationdiscussion/-/issues/3.
    /// </summary>
    public class RifleFiredWhileMoving : SinglePlayerSituation, ISinglePlayerSituation
    {
        /// <summary>
        /// Parameterless constructor required by EF Core.
        /// </summary>
        public RifleFiredWhileMoving()
        {

        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public RifleFiredWhileMoving(
            WeaponFired firstWeaponFired, 
            int bullets, 
            int inaccurateBullets
            ) : base(firstWeaponFired)
        {
            Weapon = firstWeaponFired.Weapon;
            Bullets = bullets;
            InaccurateBullets = inaccurateBullets;
        }
        public EquipmentElement Weapon { get; set; }
        public int Bullets { get; set; }
        public int InaccurateBullets { get; set; }
    }


    #region Partial definitions of metadata tables for navigational properties
    public partial class MatchEntity
    {
        public virtual ICollection<RifleFiredWhileMoving> RifleFiredWhileMoving { get; set; }
    }

    public partial class RoundEntity
    {
        public virtual ICollection<RifleFiredWhileMoving> RifleFiredWhileMoving { get; set; }
    }

    public partial class PlayerMatchEntity
    {
        public virtual ICollection<RifleFiredWhileMoving> RifleFiredWhileMoving { get; set; }
    }

    public partial class PlayerRoundEntity
    {
        public virtual ICollection<RifleFiredWhileMoving> RifleFiredWhileMoving { get; set; }
    }
    #endregion
}

