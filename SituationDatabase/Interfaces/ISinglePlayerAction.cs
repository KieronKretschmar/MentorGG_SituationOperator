using System;
using System.Collections.Generic;
using System.Text;

namespace SituationDatabase
{
    /// <summary>
    /// An Action carried out by a single player, starting at a particular time.
    /// </summary>
    public interface ISinglePlayerAction : ISituation
    {
        /// <summary>
        /// SteamId of the player who carried out the action.
        /// </summary>
        public long SteamId { get; set; }
    }
}
