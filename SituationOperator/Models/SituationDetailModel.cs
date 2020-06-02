using SituationDatabase.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SituationOperator.Models
{
    public class SituationDetailModel
    {
        public List<MatchEntity> Matches { get; set; }
        public SituationCollection SituationCollection { get; set; }

        public SituationDetailModel(List<MatchEntity> matches, SituationCollection situationCollection)
        {
            Matches = matches;
            SituationCollection = situationCollection;
        }
    }
}
