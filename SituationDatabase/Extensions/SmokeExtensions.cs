using MatchEntities;
using SituationDatabase.Helpers;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace SituationDatabase.Extensions
{
    public static class SmokeExtensions
    {
        /// <summary>
        /// Approximate radius of a smoke in CS:GO. 
        /// 
        /// This value is not exact, as smoke are not strictly circular shaped and also change their shape over time.
        /// 
        /// Taken from https://counterstrike.fandom.com/wiki/Smoke_Grenade
        /// </summary>
        public const int SMOKE_RADIUS = 144;

        /// <summary>
        /// Duration for which a smoke is fully active after detonation. 
        /// 
        /// Keep in mind that it takes 3 more seconds for a smoke to fade out completely.
        /// 
        /// Taken from https://counterstrike.fandom.com/wiki/Smoke_Grenade
        /// </summary>
        public const int THICK_SMOKE_DURATION = 15;

        public static bool BlocksLineOfSight(this Smoke smoke, Vector3 pos1, Vector3 pos2, int? time = null)
        {
            if(time != null)
            {
                if(time < smoke.Time || smoke.Time + THICK_SMOKE_DURATION < time)
                {
                    return false;
                }
            }

            var closestPointToSmokeOnTrajectory = GeometryHelper.GetClosestPointOnLineSegment(pos1, pos2, smoke.DetonationPos);

            var dist = (smoke.DetonationPos - closestPointToSmokeOnTrajectory).Length();

            return dist <= SMOKE_RADIUS;
        }
    }
}
