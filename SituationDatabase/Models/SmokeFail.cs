using System;
using System.Collections.Generic;
using System.Text;

namespace SituationDatabase.Models
{
    public class SmokeFail : Situation, ISinglePlayerSituation
    {
        public long PlayerId { get; set; }
        public long SteamId { get; set; }
        public int LineupId { get; set; }
        public string LineupName { get; set; }
    }
}
