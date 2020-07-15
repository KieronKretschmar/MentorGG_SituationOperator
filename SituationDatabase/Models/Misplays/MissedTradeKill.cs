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
    /// For more details see https://gitlab.com/mentorgg/csgo/situationdiscussion/-/issues/37.
    /// </summary>
    public class MissedTradeKill : SinglePlayerSituation, ISinglePlayerSituation
    {
        /// <summary>
        /// Parameterless constructor required by EF Core.
        /// </summary>
        public MissedTradeKill()
        {

        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public MissedTradeKill(
            long steamId,
            Kill untradedKill,
            int distanceToVictim
            ) : base(untradedKill.MatchId, untradedKill.Round, untradedKill.Time, steamId)
        {
            DistanceToVictim = distanceToVictim;
        }

        /// <summary>
        /// Distance of the player to his teammate at the moment the latter died.
        /// </summary>
        public int DistanceToVictim { get; set; }
    }


    #region Partial definitions of metadata tables for navigational properties
    public partial class MatchEntity
    {
        public virtual ICollection<MissedTradeKill> MissedTradeKill { get; set; }
    }

    public partial class RoundEntity
    {
        public virtual ICollection<MissedTradeKill> MissedTradeKill { get; set; }
    }

    public partial class PlayerMatchEntity
    {
        public virtual ICollection<MissedTradeKill> MissedTradeKill { get; set; }
    }

    public partial class PlayerRoundEntity
    {
        public virtual ICollection<MissedTradeKill> MissedTradeKill { get; set; }
    }
    #endregion
}

