using MatchEntities;
using Newtonsoft.Json;
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
        /// Parameterless constructor required by EF Core.
        /// </summary>
        public Situation()
        {

        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public Situation(long matchId, short round, int startTime)
        {
            MatchId = matchId;
            Round = round;
            StartTime = startTime;
        }

        /// <summary>
        /// Id of the Match in which the situation occured.
        /// </summary>
        public long MatchId { get; set; }

        /// <summary>
        /// Id of this situation, unique for this table and Match.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Round in which the situation occured.
        /// </summary>
        public short Round { get; set; }

        /// <summary>
        /// Time in which the situation started, referencing the entire matches timeline.
        /// </summary>
        public int StartTime { get; set; }

        /// <summary>
        /// Navigational Property.
        /// </summary>
        [JsonIgnore]
        public virtual MatchEntity Match { get; set; }

        /// <summary>
        /// Navigational Property.
        /// </summary>
        [JsonIgnore]
        public virtual RoundEntity RoundEntity { get; set; }
    }
}
