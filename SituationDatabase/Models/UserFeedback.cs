using SituationDatabase.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SituationDatabase.Models
{
    public class UserFeedback
    {
        public long MatchId { get; set; }
        public SituationType SituationType { get; set; }
        public long SituationId { get; set; }

        /// <summary>
        /// SteamId of the user who gave feedback.
        /// </summary>
        public long SteamId { get; set; }

        public bool IsPositive { get; set; }
        public string Comment { get; set; }

        public UserFeedback(long matchId, SituationType situationType, long situationId, long steamId, bool isPositive, string comment)
        {
            MatchId = matchId;
            SituationType = situationType;
            SituationId = situationId;
            SteamId = steamId;
            IsPositive = isPositive;
            Comment = comment;
        }
    }
}
