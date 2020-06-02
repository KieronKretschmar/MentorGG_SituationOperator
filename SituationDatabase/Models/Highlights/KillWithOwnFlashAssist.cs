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
    /// For more details see https://gitlab.com/mentorgg/csgo/situationdiscussion/-/issues/13.
    /// </summary>
    public class KillWithOwnFlashAssist : SinglePlayerSituation, ISinglePlayerSituation
    {
        /// <summary>
        /// Parameterless constructor required by EF Core.
        /// </summary>
        public KillWithOwnFlashAssist()
        {

        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public KillWithOwnFlashAssist(
            Flash flash, 
            int timeBetweenDetonationAndFirstKill,
            int assistedKills
            ) : base(flash)
        {
            GrenadeId = flash.GrenadeId;
            TimeBetweenDetonationAndFirstKill = timeBetweenDetonationAndFirstKill;
            AssistedKills = assistedKills;
        }

        /// <summary>
        /// Id of the Grenade this Situation is based on.
        /// </summary>
        public long GrenadeId { get; set; }

        /// <summary>
        /// Time passed between the flash detonating and the first blinded enemy dying in ms.
        /// </summary>
        public int TimeBetweenDetonationAndFirstKill { get; set; }

        /// <summary>
        /// Number of enemies the player killed that were blinded by this flash.
        /// </summary>
        public int AssistedKills { get; set; }
    }

    #region Partial definitions of metadata tables for navigational properties
    public partial class MatchEntity
    {
        public virtual ICollection<KillWithOwnFlashAssist> KillWithOwnFlashAssist { get; set; }
    }

    public partial class RoundEntity
    {
        public virtual ICollection<KillWithOwnFlashAssist> KillWithOwnFlashAssist { get; set; }
    }

    public partial class PlayerMatchEntity
    {
        public virtual ICollection<KillWithOwnFlashAssist> KillWithOwnFlashAssist { get; set; }
    }

    public partial class PlayerRoundEntity
    {
        public virtual ICollection<KillWithOwnFlashAssist> KillWithOwnFlashAssist { get; set; }
    }
    #endregion
}
