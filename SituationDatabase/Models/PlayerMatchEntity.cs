using MatchEntities;
using MatchEntities.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SituationDatabase.Models
{
    /// <summary>
    /// Metadata about a player's appearance in a match of csgo.
    /// 
    /// Tightly coupled to MatchEntities.PlayerMatchStats, so be aware of redundancy to MatchDb.
    /// </summary>
    public class PlayerMatchEntity
    {
        /// <summary>
        /// Parameterless constructor required by EF Core.
        /// </summary>
        public PlayerMatchEntity()
        {

        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public PlayerMatchEntity(PlayerMatchStats playerMatchStats)
        {
            MatchId = playerMatchStats.MatchId;
            SteamId = playerMatchStats.SteamId;
            Team = playerMatchStats.Team;
            KillCount = playerMatchStats.KillCount;
            AssistCount = playerMatchStats.AssistCount;
            DeathCount = playerMatchStats.DeathCount;
            Score = playerMatchStats.Score;
            Mvps = playerMatchStats.Mvps;
        }

        public long MatchId { get; set; }
        public long SteamId { get; set; }
        public StartingFaction Team { get; set; }
        public short KillCount { get; set; }
        public short AssistCount { get; set; }
        public short DeathCount { get; set; }
        public short Score { get; set; }
        public short Mvps { get; set; }

        #region Navigational Properties
        public virtual MatchEntity Match { get; set; }
        public virtual ICollection<RoundEntity> Round { get; set; }
        public virtual ICollection<PlayerRoundEntity> PlayerRound { get; set; }
        #endregion
    }
}
