﻿using MatchEntities;
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
    /// For more details see https://gitlab.com/mentorgg/csgo/situationdiscussion/-/issues/36.
    /// </summary>
    public class FlashAssist : SinglePlayerSituation, ISinglePlayerSituation
    {
        /// <summary>
        /// Parameterless constructor required by EF Core.
        /// </summary>
        public FlashAssist()
        {

        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public FlashAssist(
            Flash flash,
            int flashedTeammates,
            int timeFlashedTeammates,
            int flashedTeammatesDeaths,
            int timeFlashedEnemies
            ) : base(flash)
        {
            GrenadeId = flash.GrenadeId;
            FlashedTeammates = flashedTeammates;
            TimeFlashedTeammates = timeFlashedTeammates;
            FlashedTeammatesDeaths = flashedTeammatesDeaths;
            TimeFlashedEnemies = timeFlashedEnemies;
        }


        /// <summary>
        /// Id of the Grenade this Situation is based on.
        /// </summary>
        public long GrenadeId { get; set; }
        public int FlashedTeammates { get; set; }
        public int TimeFlashedTeammates { get; set; }
        public int FlashedTeammatesDeaths { get; set; }
        public int TimeFlashedEnemies { get; set; }
    }

    #region Partial definitions of metadata tables for navigational properties
    public partial class MatchEntity
    {
        public virtual ICollection<FlashAssist> FlashAssist { get; set; }
    }

    public partial class RoundEntity
    {
        public virtual ICollection<FlashAssist> FlashAssist { get; set; }
    }

    public partial class PlayerMatchEntity
    {
        public virtual ICollection<FlashAssist> FlashAssist { get; set; }
    }

    public partial class PlayerRoundEntity
    {
        public virtual ICollection<FlashAssist> FlashAssist { get; set; }
    }
    #endregion
}
