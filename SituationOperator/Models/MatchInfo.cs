using SituationDatabase.Models;
using SituationOperator.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SituationOperator.Models
{
    public class MatchInfo
    {
        public long MatchId { get; set; }
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
        /// <param name="subscriptionType"></param>
        public MatchInfo(MatchEntity matchEntity, SubscriptionType subscriptionType)
        {
            MatchId = matchEntity.MatchId;
            TotalRounds = matchEntity.Rounds;
            Map = matchEntity.Map;
            MatchDate = matchEntity.MatchDate;
            switch (subscriptionType)
            {
                case SubscriptionType.Free:
                    // Free users get first half
                    AllowedRounds = Enumerable.Range(1, Math.Min(15, matchEntity.Rounds)).ToList();
                    break;
                case SubscriptionType.Premium:
                case SubscriptionType.Ultimate:
                case SubscriptionType.Influencer:
                    // All other users get all rounds
                    AllowedRounds = Enumerable.Range(1, matchEntity.Rounds).ToList();
                    break;
                default:
                    break;
            }
        }
    }
}
