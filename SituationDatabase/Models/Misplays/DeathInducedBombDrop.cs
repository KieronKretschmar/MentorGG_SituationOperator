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
    /// For more details see https://gitlab.com/mentorgg/csgo/situationdiscussion/-/issues/1.
    /// </summary>
    public class DeathInducedBombDrop : SinglePlayerSituation, ISinglePlayerSituation
    {
        /// <summary>
        /// Parameterless constructor required by EF Core.
        /// </summary>
        public DeathInducedBombDrop()
        {

        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public DeathInducedBombDrop(
            ItemDropped bombDrop, 
            int pickedUpAfter, 
            int teammatesAlive, 
            float closestTeammateDistance
            ) : base(bombDrop.MatchId, bombDrop.Round, bombDrop.Time, bombDrop.PlayerId)
        {
            PickedUpAfter = pickedUpAfter;
            TeammatesAlive = teammatesAlive;
            ClosestTeammateDistance = closestTeammateDistance;
        }

        /// <summary>
        /// Time after which the bomb was picked up.
        /// Defaults to -1 if it was not picked up during the round.
        /// </summary>
        public int PickedUpAfter { get; set; }

        /// <summary>
        /// Number of teammates that were alive when the player died.
        /// </summary>
        public int TeammatesAlive { get; set; }

        /// <summary>
        /// Distance to the closes teammate
        /// </summary>
        public float ClosestTeammateDistance { get; set; }
    }


    #region Partial definitions of metadata tables for navigational properties
    public partial class MatchEntity
    {
        public virtual ICollection<DeathInducedBombDrop> DeathInducedBombDrop { get; set; }
    }

    public partial class RoundEntity
    {
        public virtual ICollection<DeathInducedBombDrop> DeathInducedBombDrop { get; set; }
    }

    public partial class PlayerMatchEntity
    {
        public virtual ICollection<DeathInducedBombDrop> DeathInducedBombDrop { get; set; }
    }

    public partial class PlayerRoundEntity
    {
        public virtual ICollection<DeathInducedBombDrop> DeathInducedBombDrop { get; set; }
    }
    #endregion
}

