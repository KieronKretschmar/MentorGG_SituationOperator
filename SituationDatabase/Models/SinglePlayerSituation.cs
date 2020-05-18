using MatchEntities.Interfaces;
using Newtonsoft.Json;
using SituationDatabase.Extensions;
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

        /// <summary>
        /// Constructor using data from an IPlayerEvent.
        /// </summary>
        public SinglePlayerSituation(IPlayerEvent playerEvent) : base(playerEvent)
        {
            SteamId = playerEvent.PlayerId;
        }

        /// <summary>
        /// Constructor using data from an IGrenadeEvent, with the Time property being set as the ThrowTime.
        /// </summary>
        public SinglePlayerSituation(IGrenadeEvent grenadeThrowEvent) 
            : base(grenadeThrowEvent.MatchId, grenadeThrowEvent.Round, grenadeThrowEvent.GetThrowTime())
        {
            SteamId = grenadeThrowEvent.PlayerId;
        }

        /// <inheritdoc/>
        public long SteamId { get; set; }

        /// <summary>
        /// Navigational Property.
        /// </summary>
        [JsonIgnore]
        public virtual PlayerMatchEntity PlayerMatch { get; set; }

        /// <summary>
        /// Navigational Property.
        /// </summary>
        [JsonIgnore]
        public virtual PlayerRoundEntity PlayerRound { get; set; }
    }
}
