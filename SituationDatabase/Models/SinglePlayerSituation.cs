using System;
using System.Collections.Generic;
using System.Text;

namespace SituationDatabase.Models
{
    public class SinglePlayerSituation : Situation, ISinglePlayerSituation
    {
        /// <summary>
        /// Parameterless constructor required by EF Core.
        /// </summary>
        public SinglePlayerSituation()
        {

        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public SinglePlayerSituation(long matchId, short round, int startTime, long steamId) : base(matchId, round, startTime)
        {
            SteamId = steamId;
        }

        /// <inheritdoc/>
        public long SteamId { get; set; }
    }
}
