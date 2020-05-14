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
    public class TeamFlash : SinglePlayerSituation, ISinglePlayerSituation
    {
        /// <summary>
        /// Parameterless constructor required by EF Core.
        /// </summary>
        public TeamFlash()
        {

        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public TeamFlash(
            Flash flash,
            int flashedTeammates,
            int timeFlashedTeammates,
            int flashedTeammatesDeaths,
            int timeFlashedEnemies
            ) : base(flash.MatchId, flash.Round, TrajectoryHelper.GetThrowTime(flash), flash.PlayerId)
        {
            FlashedTeammates = flashedTeammates;
            FimeFlashedTeammates = timeFlashedTeammates;
            FlashedTeammatesDeaths = flashedTeammatesDeaths;
            TimeFlashedEnemies = timeFlashedEnemies; 
        }

        public int FlashedTeammates { get; set; }
        public int FimeFlashedTeammates { get; set; }
        public int FlashedTeammatesDeaths { get; set; }
        public int TimeFlashedEnemies { get; set; }
    }


    #region Partial definitions of metadata tables for navigational properties
    public partial class MatchEntity
    {
        public virtual ICollection<TeamFlash> TeamFlash { get; set; }
    }

    public partial class RoundEntity
    {
        public virtual ICollection<TeamFlash> TeamFlash { get; set; }
    }

    public partial class PlayerMatchEntity
    {
        public virtual ICollection<TeamFlash> TeamFlash { get; set; }
    }

    public partial class PlayerRoundEntity
    {
        public virtual ICollection<TeamFlash> TeamFlash { get; set; }
    }
    #endregion
}

