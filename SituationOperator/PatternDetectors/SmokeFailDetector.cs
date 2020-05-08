using MatchEntities;
using SituationDatabase;
using SituationDatabase.Models;
using SituationOperator.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SituationOperator.PatternDetectors
{
    public class SmokeFailDetector : IPatternDetector<SmokeFail>
    {
        public Task<IEnumerable<SmokeFail>> ExtractSituations(MatchDataSet matchData)
        {
            var myList = new List<SmokeFail>
            {
                new SmokeFail
                {
                    MatchId = 1,
                    LineupId = 1
                }
            };
            return Task.FromResult(myList.AsEnumerable());
        }
    }
}
