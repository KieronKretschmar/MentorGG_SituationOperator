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
    /// For more details see https://gitlab.com/mentorgg/csgo/situationdiscussion/-/issues/21.
    /// </summary>
    public class HasNotBoughtDefuseKit : SinglePlayerSituation, ISinglePlayerSituation
    {
        /// <summary>
        /// Parameterless constructor required by EF Core.
        /// </summary>
        public HasNotBoughtDefuseKit()
        {

        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public HasNotBoughtDefuseKit(
            RoundStats roundStats,
            long steamId,
            int defuseKitsInTeam,
            int moneyLeft,
            int teamEquipmentValue,
            int playerEquipmentValue
            ) : base(roundStats.MatchId, roundStats.Round, roundStats.StartTime, steamId)
        {
            DefuseKitsInTeam = defuseKitsInTeam;
            MoneyLeft = moneyLeft;
            TeamEquipmentValue = teamEquipmentValue;
            PlayerEquipmentValue = playerEquipmentValue;
        }

        /// <summary>
        /// Amount of money the player still had available after finishing his buys.
        /// </summary>
        public int MoneyLeft { get; set; }

        /// <summary>
        /// Total value of the equipment the player's team carried.
        /// </summary>
        public int TeamEquipmentValue { get; }

        /// <summary>
        /// Total value of the equipment the player carried this round.
        /// </summary>
        public int PlayerEquipmentValue { get; }

        /// <summary>
        /// Number of teammates that bought a defusekit in this round.
        /// </summary>
        public int DefuseKitsInTeam { get; set; }
    }


    #region Partial definitions of metadata tables for navigational properties
    public partial class MatchEntity
    {
        public virtual ICollection<HasNotBoughtDefuseKit> HasNotBoughtDefuseKit { get; set; }
    }

    public partial class RoundEntity
    {
        public virtual ICollection<HasNotBoughtDefuseKit> HasNotBoughtDefuseKit { get; set; }
    }

    public partial class PlayerMatchEntity
    {
        public virtual ICollection<HasNotBoughtDefuseKit> HasNotBoughtDefuseKit { get; set; }
    }

    public partial class PlayerRoundEntity
    {
        public virtual ICollection<HasNotBoughtDefuseKit> HasNotBoughtDefuseKit { get; set; }
    }
    #endregion
}

