using SituationDatabase;
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
        public long MatchId { get; set; }
        public List<List<ISituation>> Misplays { get; set; }
        public List<List<ISituation>> Highlights { get; set; }

        public MatchSituationsModel()
        {
            Misplays = new List<List<ISituation>>();
            Highlights = new List<List<ISituation>>();
        }
    }
}
