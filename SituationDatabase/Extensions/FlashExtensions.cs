using MatchEntities;
using SituationDatabase.Helpers;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace SituationDatabase.Extensions
{
    public static class FlashExtensions
    {
        /// <summary>
        /// Upper bound for time a flash can have an effect on a single player.
        /// 
        /// See https://counterstrike.fandom.com/wiki/Flashbang
        /// </summary>
        public const int MAX_FLASH_TIME = 6000;
    }
}
