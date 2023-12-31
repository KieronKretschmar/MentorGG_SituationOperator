﻿using MatchEntities;
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
    /// For more details see https://gitlab.com/mentorgg/csgo/situationdiscussion/-/issues/2.
    /// </summary>
    public class PushBeforeSmokeDetonated : SinglePlayerSituation, ISinglePlayerSituation
    {
        /// <summary>
        /// Parameterless constructor required by EF Core.
        /// </summary>
        public PushBeforeSmokeDetonated()
        {

        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public PushBeforeSmokeDetonated(
            Smoke smoke,
            Damage damageTaken 
            ) : base(damageTaken.MatchId, damageTaken.Round, damageTaken.Time, damageTaken.VictimId)
        {
            GrenadeId = smoke.GrenadeId;
            SmokeDetonationTime = smoke.GetDetonationTime();
        }

        /// <summary>
        /// Id of the Grenade this Situation is based on.
        /// </summary>
        public long GrenadeId { get; set; }

        /// <summary>
        /// Time at which the smoke detonated.
        /// </summary>
        public int SmokeDetonationTime { get; set; }
    }


    #region Partial definitions of metadata tables for navigational properties
    public partial class MatchEntity
    {
        public virtual ICollection<PushBeforeSmokeDetonated> PushBeforeSmokeDetonated { get; set; }
    }

    public partial class RoundEntity
    {
        public virtual ICollection<PushBeforeSmokeDetonated> PushBeforeSmokeDetonated { get; set; }
    }

    public partial class PlayerMatchEntity
    {
        public virtual ICollection<PushBeforeSmokeDetonated> PushBeforeSmokeDetonated { get; set; }
    }

    public partial class PlayerRoundEntity
    {
        public virtual ICollection<PushBeforeSmokeDetonated> PushBeforeSmokeDetonated { get; set; }
    }
    #endregion
}

