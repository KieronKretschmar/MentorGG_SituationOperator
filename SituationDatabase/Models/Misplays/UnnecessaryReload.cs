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
    /// For more details see https://gitlab.com/mentorgg/csgo/situationdiscussion/-/issues/7.
    /// </summary>
    public class UnnecessaryReload : SinglePlayerSituation, ISinglePlayerSituation
    {
        /// <summary>
        /// Parameterless constructor required by EF Core.
        /// </summary>
        public UnnecessaryReload()
        {

        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public UnnecessaryReload(
            WeaponReload weaponReload
            ) : base(weaponReload)
        {
            Weapon = weaponReload.Weapon;
            AmmoBefore = weaponReload.AmmoBefore;
        }
        public EquipmentElement Weapon { get; set; }
        public int AmmoBefore { get; set; }
    }


    #region Partial definitions of metadata tables for navigational properties
    public partial class MatchEntity
    {
        public virtual ICollection<UnnecessaryReload> UnnecessaryReload { get; set; }
    }

    public partial class RoundEntity
    {
        public virtual ICollection<UnnecessaryReload> UnnecessaryReload { get; set; }
    }

    public partial class PlayerMatchEntity
    {
        public virtual ICollection<UnnecessaryReload> UnnecessaryReload { get; set; }
    }

    public partial class PlayerRoundEntity
    {
        public virtual ICollection<UnnecessaryReload> UnnecessaryReload { get; set; }
    }
    #endregion
}

