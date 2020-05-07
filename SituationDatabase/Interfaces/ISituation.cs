using System;
using System.Collections.Generic;
using System.Text;

namespace SituationDatabase
{
    /// <summary>
    /// A Situation that occured in a specific round and started at a particular time.
    /// </summary>
    public interface ISituation
    {
        /// <summary>
        /// Id of the Match in which the situation occured.
        /// </summary>
        public long MatchId { get; set; }

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
