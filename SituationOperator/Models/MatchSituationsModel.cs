using SituationDatabase;
using SituationDatabase.Models;
using SituationOperator.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SituationOperator.Models
{
    /// <summary>
    /// Contains all Situations from a particular match.
    /// </summary>
    public class MatchSituationsModel
    {
        /// <summary>
        /// MatchId of the requested match.
        /// </summary>
        public long MatchId { get; set; }

        /// <summary>
        /// List of SituationCollections, one for each type of misplay.
        /// </summary>
        public List<SituationCollection> Misplays { get; set; }

        /// <summary>
        /// List of SituationCollections, one for each type of highlight.
        /// </summary>
        public List<SituationCollection> Highlights { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="matchId"></param>
        public MatchSituationsModel(long matchId)
        {
            MatchId = matchId;
            Misplays = new List<SituationCollection>();
            Highlights = new List<SituationCollection>();
        }

        /// <summary>
        /// Holds a collection of a particular type of Situations.
        /// </summary>
        public class SituationCollection
        {
            /// <summary>
            /// Identifies the type of situation.
            /// </summary>
            public SituationType SituationType { get; set; }

            /// <summary>
            /// Name of the type of situation.
            /// </summary>
            public string SituationName { get; set; }

            /// <summary>
            /// Collection of situations.
            /// </summary>
            public List<ISituation> Situations { get; set; }

            /// <summary>
            /// Constructor.
            /// </summary>
            public SituationCollection(SituationType situationType, IEnumerable<ISituation> situations)
            {
                SituationType = situationType;
                SituationName = situationType.ToString();
                Situations = situations.ToList();
            }
        }
    }
}
