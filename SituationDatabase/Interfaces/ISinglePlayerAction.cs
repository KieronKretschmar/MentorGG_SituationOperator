using System;
using System.Collections.Generic;
using System.Text;

namespace SituationDatabase
{
    /// <summary>
    /// An Action carried out by a single player, starting at a particular time.
    /// </summary>
    public interface ISinglePlayerSituation : ISituation
    {
        /// <summary>
        /// SteamId of the acting player.
        /// </summary>
        public long SteamId { get; set; }
    }
}
