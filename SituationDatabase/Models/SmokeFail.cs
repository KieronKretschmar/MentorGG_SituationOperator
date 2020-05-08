using System;
using System.Collections.Generic;
using System.Text;

namespace SituationDatabase.Models
{
    public class SmokeFail : ISinglePlayerAction, ISituation
    {
        public long MatchId { get; set; }
        public int Id { get; set; }
        public string Map { get; set; }
        public DateTime MatchDate { get; set; }
        public long PlayerId { get; set; }
        public short Round { get; set; }
        public int Time { get; set; }
        public int StartTime { get; set; }

        public long SteamId { get; set; }

        public int LineupId { get; set; }
        public string LineupName { get; set; }
    }
}
