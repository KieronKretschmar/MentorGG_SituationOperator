using MatchEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SituationDatabase.Models
{
    /// <summary>
    /// A Situation. 
    /// 
    /// For more details see the corresponding ISituationManager in SituationOperator.
    /// </summary>
    public class SmokeFail : SinglePlayerSituation, ISinglePlayerSituation
    {
        /// <summary>
        /// Parameterless constructor required by EF Core.
        /// </summary>
        public SmokeFail()
        {

        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public SmokeFail(Smoke smoke) : base(smoke.MatchId, smoke.Round, smoke.GetThrowTime(), smoke.PlayerId)
        {
            LineupId = smoke.LineUp;

            //TODO: Set LineupName, e.g. in SituationManager.EnrichData()
        }

        public int LineupId { get; set; }
        public string LineupName { get; set; }
    }


    #region Partial definitions of metadata tables for navigational properties
    public partial class MatchEntity
    {
        public virtual ICollection<SmokeFail> SmokeFail { get; set; }
    }

    public partial class RoundEntity
    {
        public virtual ICollection<SmokeFail> SmokeFail { get; set; }
    }

    public partial class PlayerMatchEntity
    {
        public virtual ICollection<SmokeFail> SmokeFail { get; set; }
    }

    public partial class PlayerRoundEntity
    {
        public virtual ICollection<SmokeFail> SmokeFail { get; set; }
    }
    #endregion
}

