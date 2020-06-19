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
    /// For more details see https://gitlab.com/mentorgg/csgo/situationdiscussion/-/issues/35.
    /// </summary>
    public class TradeKill : SinglePlayerSituation, ISinglePlayerSituation
    {
        /// <summary>
        /// Parameterless constructor required by EF Core.
        /// </summary>
        public TradeKill()
        {

        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public TradeKill(
            Kill originalKill,
            Kill tradeKill
            ) : base(tradeKill.MatchId, tradeKill.Round, originalKill.Time, tradeKill.PlayerId)
        {
            TimeBetweenKills = tradeKill.Time - originalKill.Time;
        }

        /// <summary>
        /// Time until the original kill was traded.
        /// </summary>
        public int TimeBetweenKills { get; set; }
    }

    #region Partial definitions of metadata tables for navigational properties
    public partial class MatchEntity
    {
        public virtual ICollection<TradeKill> TradeKill { get; set; }
    }

    public partial class RoundEntity
    {
        public virtual ICollection<TradeKill> TradeKill { get; set; }
    }

    public partial class PlayerMatchEntity
    {
        public virtual ICollection<TradeKill> TradeKill { get; set; }
    }

    public partial class PlayerRoundEntity
    {
        public virtual ICollection<TradeKill> TradeKill { get; set; }
    }
    #endregion
}
