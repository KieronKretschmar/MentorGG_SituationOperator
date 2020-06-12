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
    /// For more details see https://gitlab.com/mentorgg/csgo/situationdiscussion/-/issues/30.
    /// </summary>
    public class Clutch : SinglePlayerSituation, ISinglePlayerSituation
    {
        /// <summary>
        /// Parameterless constructor required by EF Core.
        /// </summary>
        public Clutch()
        {

        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public Clutch(
            RoundStats roundStats,
            long steamId,
            int clutchStartTime,
            int enemiesAlive
            ) : base(roundStats.MatchId, roundStats.Round, clutchStartTime, steamId)
        {
            EnemiesAlive = enemiesAlive;
            WinType = roundStats.WinType;
        }

        /// <summary>
        /// Number of enemies alive when the player's last teammate died.
        /// </summary>
        public int EnemiesAlive { get; set; }

        /// <summary>
        /// The round's WinType.
        /// </summary>
        public WinType WinType { get; set; }
    }

    #region Partial definitions of metadata tables for navigational properties
    public partial class MatchEntity
    {
        public virtual ICollection<Clutch> Clutch { get; set; }
    }

    public partial class RoundEntity
    {
        public virtual ICollection<Clutch> Clutch { get; set; }
    }

    public partial class PlayerMatchEntity
    {
        public virtual ICollection<Clutch> Clutch { get; set; }
    }

    public partial class PlayerRoundEntity
    {
        public virtual ICollection<Clutch> Clutch { get; set; }
    }
    #endregion
}
