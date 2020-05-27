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
    /// For more details see https://gitlab.com/mentorgg/csgo/situationdiscussion/-/issues/4.
    /// </summary>
    public class SelfFlash : SinglePlayerSituation, ISinglePlayerSituation
    {
        /// <summary>
        /// Parameterless constructor required by EF Core.
        /// </summary>
        public SelfFlash()
        {

        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public SelfFlash(
            Flash flash,
            int timeFlashedSelf,
            int? deathTimeSelf,
            int timeFlashedEnemies,
            int angleToCrosshairSelf
            ) : base(flash)
        {
            GrenadeId = flash.GrenadeId;
            TimeFlashedSelf = timeFlashedSelf;
            DeathTimeSelf = deathTimeSelf;
            TimeFlashedEnemies = timeFlashedEnemies;
            AngleToCrosshairSelf = angleToCrosshairSelf; 
        }

        /// <summary>
        /// Id of the Grenade this Situation is based on.
        /// </summary>
        public long GrenadeId { get; set; }
        public int TimeFlashedSelf { get; set; }
        public int TimeFlashedEnemies { get; set; }
        public int AngleToCrosshairSelf { get; set; }
        public int? DeathTimeSelf { get; set; }
    }


    #region Partial definitions of metadata tables for navigational properties
    public partial class MatchEntity
    {
        public virtual ICollection<SelfFlash> SelfFlash { get; set; }
    }

    public partial class RoundEntity
    {
        public virtual ICollection<SelfFlash> SelfFlash { get; set; }
    }

    public partial class PlayerMatchEntity
    {
        public virtual ICollection<SelfFlash> SelfFlash { get; set; }
    }

    public partial class PlayerRoundEntity
    {
        public virtual ICollection<SelfFlash> SelfFlash { get; set; }
    }
    #endregion
}

