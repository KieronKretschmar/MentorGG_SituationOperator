using System;
using System.Collections.Generic;
using System.Text;

namespace SituationDatabase.Models
{
    public class SituationInfoByRank
    {
        public int AvgRank { get; set; }
        public int RoundCount { get; set; }
        public int SituationCount { get; set; }

        public double SituationsPerRound => (double)SituationCount / RoundCount;
    }
}
