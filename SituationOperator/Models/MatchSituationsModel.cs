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
        public long MatchId { get; set; }
        public List<SituationCollection> Misplays { get; set; }
        public List<SituationCollection> Highlights { get; set; }

        public MatchSituationsModel(long matchId)
        {
            MatchId = matchId;
            Misplays = new List<SituationCollection>();
            Highlights = new List<SituationCollection>();
        }


        public class SituationCollection
        {
            public SituationType SituationType { get; set; }

            public string SituationName { get; set; }

            public List<ISituation> Situations { get; set; }

            public SituationCollection(SituationType situationType, IEnumerable<ISituation> situations)
            {
                SituationType = situationType;
                SituationName = situationType.ToString();
                Situations = situations.ToList();
            }
        }
    }
}
