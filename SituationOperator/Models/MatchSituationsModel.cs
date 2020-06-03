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
        /// Metadata of the requested match.
        /// </summary>
        public MatchInfo Match { get; set; }

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
        /// <param name="match"></param>
        public MatchSituationsModel(MatchInfo match)
        {
            Match = match;
            Misplays = new List<SituationCollection>();
            Highlights = new List<SituationCollection>();
        }        
    }
}
