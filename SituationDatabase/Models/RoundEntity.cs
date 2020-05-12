using MatchEntities;
using MatchEntities.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SituationDatabase.Models
{
    /// <summary>
    /// Metadata about a round of a match of csgo.
    /// 
    /// Tightly coupled to MatchEntities.RoundStats, so be aware of redundancy to MatchDb.
    /// </summary>
    public class RoundEntity
    {
        /// <summary>
        /// Parameterless constructor required by EF Core.
        /// </summary>
        public RoundEntity()
        {

        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public RoundEntity(RoundStats roundStats)
        {
            MatchId = roundStats.MatchId;
            Round = roundStats.Round;
            WinnerTeam = roundStats.WinnerTeam;
            OriginalSide = roundStats.OriginalSide;
            BombPlanted = roundStats.BombPlanted;
            WinType = roundStats.WinType;
            RoundTime = roundStats.RoundTime;
            StartTime = roundStats.StartTime;
            EndTime = roundStats.EndTime;
            RealEndTime = roundStats.RealEndTime;
        }

        public long MatchId { get; set; }
        public short Round { get; set; }
        public StartingFaction WinnerTeam { get; set; }
        public bool OriginalSide { get; set; }
        public bool BombPlanted { get; set; }
        public byte WinType { get; set; }
        public int RoundTime { get; set; }
        public int StartTime { get; set; }
        public int EndTime { get; set; }
        public int RealEndTime { get; set; }

        #region Navigational Properties
        public virtual MatchEntity Match { get; set; }
        public virtual PlayerMatchEntity PlayerMatch { get; set; }
        public virtual ICollection<PlayerRoundEntity> PlayerRound { get; set; }
        #endregion
    }
}

