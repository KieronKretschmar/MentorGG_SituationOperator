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
    /// For more details see https://gitlab.com/mentorgg/csgo/situationdiscussion/-/issues/29.
    /// </summary>
    public class HighImpactRound : SinglePlayerSituation, ISinglePlayerSituation
    {
        /// <summary>
        /// Parameterless constructor required by EF Core.
        /// </summary>
        public HighImpactRound()
        {

        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public HighImpactRound(
            RoundStats roundStats,
            long steamId,
            List<Kill> kills,
            int damageDealt
            ) : base(roundStats.MatchId, roundStats.Round, kills.First().Time, steamId)
        {
            Kills = kills.Count();
            DamageDealt = damageDealt;
        }

        /// <summary>
        /// Number of enemies alive when the player's last teammate died.
        /// </summary>
        public int Kills { get; set; }

        /// <summary>
        /// Number of enemies alive when the player's last teammate died.
        /// </summary>
        public int DamageDealt { get; set; }
    }

    #region Partial definitions of metadata tables for navigational properties
    public partial class MatchEntity
    {
        public virtual ICollection<HighImpactRound> HighImpactRound { get; set; }
    }

    public partial class RoundEntity
    {
        public virtual ICollection<HighImpactRound> HighImpactRound { get; set; }
    }

    public partial class PlayerMatchEntity
    {
        public virtual ICollection<HighImpactRound> HighImpactRound { get; set; }
    }

    public partial class PlayerRoundEntity
    {
        public virtual ICollection<HighImpactRound> HighImpactRound { get; set; }
    }
    #endregion
}
