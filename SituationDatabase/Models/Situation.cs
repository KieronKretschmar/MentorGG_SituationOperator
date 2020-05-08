using System;
using System.Collections.Generic;
using System.Text;

namespace SituationDatabase.Models
{
    /// <summary>
    /// Basic implementation of ISituation
    /// </summary>
    public abstract class Situation : ISituation
    {
        /// <summary>
        /// Id of the Match in which the situation occured.
        /// </summary>
        public long MatchId { get; set; }

        /// <summary>
        /// Id of this situation, unique for this table and Match.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Map on which the match was played.
        /// </summary>
        public string Map { get; set; }

        /// <summary>
        /// DateTime when the match was played.
        /// </summary>
        public DateTime MatchDate { get; set; }

        /// <summary>
        /// Round in which the situation occured.
        /// </summary>
        public short Round { get; set; }

        /// <summary>
        /// Time in which the situation started, referencing the entire matches timeline.
        /// </summary>
        public int StartTime { get; set; }
    }
}
