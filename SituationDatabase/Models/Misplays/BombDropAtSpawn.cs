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
    /// For more details see https://gitlab.com/mentorgg/csgo/situationdiscussion/-/issues/26.
    /// </summary>
    public class BombDropAtSpawn : SinglePlayerSituation, ISinglePlayerSituation
    {
        /// <summary>
        /// Parameterless constructor required by EF Core.
        /// </summary>
        public BombDropAtSpawn()
        {

        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public BombDropAtSpawn(
            ItemDropped bombDrop, 
            int pickedUpAfter
            ) : base(bombDrop.MatchId, bombDrop.Round, bombDrop.Time, bombDrop.PlayerId)
        {
            PickedUpAfter = pickedUpAfter;
        }

        /// <summary>
        /// Time after which the bomb was picked up.
        /// Defaults to -1 if it was not picked up during the round.
        /// </summary>
        public int PickedUpAfter { get; set; }
    }


    #region Partial definitions of metadata tables for navigational properties
    public partial class MatchEntity
    {
        public virtual ICollection<BombDropAtSpawn> BombDropAtSpawn { get; set; }
    }

    public partial class RoundEntity
    {
        public virtual ICollection<BombDropAtSpawn> BombDropAtSpawn { get; set; }
    }

    public partial class PlayerMatchEntity
    {
        public virtual ICollection<BombDropAtSpawn> BombDropAtSpawn { get; set; }
    }

    public partial class PlayerRoundEntity
    {
        public virtual ICollection<BombDropAtSpawn> BombDropAtSpawn { get; set; }
    }
    #endregion
}

