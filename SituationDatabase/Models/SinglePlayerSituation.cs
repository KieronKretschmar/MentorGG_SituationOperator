using System;
using System.Collections.Generic;
using System.Text;

namespace SituationDatabase.Models
{
    public class SinglePlayerSituation : Situation, ISinglePlayerSituation
    {
        public SinglePlayerSituation(long matchId, short round, int startTime, long steamId) : base(matchId, round, startTime)
        {
            SteamId = steamId;
        }

        public long SteamId { get; set; }
    }
}
