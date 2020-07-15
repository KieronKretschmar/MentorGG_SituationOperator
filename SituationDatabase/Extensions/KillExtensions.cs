using MatchEntities;
using SituationDatabase.Helpers;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace SituationDatabase.Extensions
{
    public static class KillExtensions
    {
        public static bool ThroughCover(this Kill kill)
        {
            return kill.KillType == MatchEntities.Enums.KillType.CoverBodyShot
                || kill.KillType == MatchEntities.Enums.KillType.CoverHeadShot;
        }

        public static float Distance(this Kill kill)
        {
            return (kill.PlayerPos - kill.VictimPos).Length();
        }
    }
}
