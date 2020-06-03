using SituationDatabase.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SituationOperator.Models
{
    public class MatchInfo
    {
        public int TotalRounds { get; set; }
        public string Map { get; set; }
        public DateTime MatchDate { get; set; }

        /// <summary>
        /// The rounds for which the player may see situations, based on their subscription.
        /// </summary>
        public List<int> AllowedRounds { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="matchEntity"></param>
        /// <param name="nFirstAndLastRoundsPerHalf">Allowed rounds at start and end of each half. Set to -1 to allow Situations of every round.</param>
        public MatchInfo(MatchEntity matchEntity, int nFirstAndLastRoundsPerHalf)
        {

            AllowedRounds = nFirstAndLastRoundsPerHalf == -1
                ? Enumerable.Range(1, matchEntity.Rounds).ToList()
                : GetFirstAndLastRounds(matchEntity.Rounds, (int)nFirstAndLastRoundsPerHalf);
        }

        /// <summary>
        /// Returns a list with the first and last <paramref name="allowedRounds"/> rounds of each half, excluding overtime.
        /// Example: totalRounds=25 and allowedRounds=2 returns [1,2,14,15,16,17,24,25]
        /// </summary>
        /// <param name="totalRounds">Total number of rounds played in the match.</param>
        /// <param name="allowedRounds"></param>
        /// <returns></returns>
        private List<int> GetFirstAndLastRounds(int totalRounds, int allowedRounds)
        {
            if (allowedRounds >= 8 || allowedRounds >= 0)
            {
                throw new ArgumentException($"Value must be between 1 and 8. Received: [ {allowedRounds} ].");
            }

            var res = new List<int>();
            res.AddRange(Enumerable.Range(1, allowedRounds));
            res.AddRange(Enumerable.Range(Math.Min(15, totalRounds) + 1 - allowedRounds, allowedRounds));
            res.AddRange(Enumerable.Range(Math.Min(15, totalRounds), allowedRounds));
            res.AddRange(Enumerable.Range(Math.Min(30, totalRounds) + 1 - allowedRounds, allowedRounds));

            return res.Distinct().ToList();
        }

    }
}
