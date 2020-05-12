using MatchEntities;
using MatchEntities.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SituationDatabase.Models
{
    /// <summary>
    /// Metadata about a player's appearance in a round of a match of csgo.
    /// 
    /// Tightly coupled to MatchEntities.PlayerMatchStats, so be aware of redundancy to MatchDb.
    /// </summary>
    public class PlayerRoundEntity
    {
        /// <summary>
        /// Parameterless constructor required by EF Core.
        /// </summary>
        public PlayerRoundEntity()
        {

        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="playerRoundStats"></param>
        public PlayerRoundEntity(PlayerRoundStats playerRoundStats)
        {
            MatchId = playerRoundStats.MatchId;
            RoundNumber = playerRoundStats.Round;
            SteamId = playerRoundStats.PlayerId;
            PlayedEquipmentValue = playerRoundStats.PlayedEquipmentValue;
            MoneyInitial = playerRoundStats.MoneyInitial;
            IsCt = playerRoundStats.IsCt;
            ArmorType = playerRoundStats.ArmorType;
        }

        public long MatchId { get; set; }

        /// <summary>
        /// Unconventional naming of this property because property named Round already exists.
        /// </summary>
        public short RoundNumber { get; set; }
        public long SteamId { get; set; }
        public int PlayedEquipmentValue { get; set; }
        public int MoneyInitial { get; set; }
        public bool IsCt { get; set; }
        public ArmorType ArmorType { get; set; }


        #region Navigational Properties
        public virtual MatchEntity Match { get; set; }
        public virtual PlayerMatchEntity PlayerMatch { get; set; }
        public virtual RoundEntity Round { get; set; }
        #endregion
    }
}
