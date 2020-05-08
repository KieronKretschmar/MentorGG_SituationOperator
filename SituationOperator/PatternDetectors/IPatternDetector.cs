using MatchEntities;
using SituationDatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SituationOperator.PatternDetectors
{
    /// <summary>
    /// Extracts Situations from match data that follow a specific pattern.
    /// </summary>
    /// <typeparam name="TSituation"> The type of Situation that is detected.</typeparam>
    public interface IPatternDetector<TSituation> where TSituation : ISituation
    {
        /// <summary>
        /// Returns situations that implement the specific pattern.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        Task<IEnumerable<TSituation>> ExtractSituations(MatchDataSet data);
    }
}
