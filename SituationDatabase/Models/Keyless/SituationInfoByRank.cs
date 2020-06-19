using System;
using System.Collections.Generic;
using System.Text;

namespace SituationDatabase.Models
{
    public class SituationInfoByRank
    {
        public int RankBeforeMatch { get; set; }
        public int PlayerRoundCount { get; set; }
        public int SituationCount { get; set; }

        public double SituationsPerPlayerAndRound => (double)SituationCount / PlayerRoundCount;
    }
}
