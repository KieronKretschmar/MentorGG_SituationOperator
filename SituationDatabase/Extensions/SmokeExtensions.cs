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
        public const int THICK_SMOKE_DURATION = 15000;

        /// <summary>
        /// Indicates whether the direct connection between two positions was blocked by this smoke.
        /// </summary>
        /// <param name="smoke"></param>
        /// <param name="pos1">Position of the first object or player</param>
        /// <param name="pos2">Position of the second object or player</param>
        /// <param name="time">If null, assumes that the smoke is still fully active at the time.</param>
        /// <param name="isDucking"></param>
        /// <returns></returns>
        public static bool BlocksLineOfSight(this Smoke smoke, Vector3 pos1, Vector3 pos2, int? time = null, bool? isDucking = null)
        {
            if (time != null)
            {
                if (time < smoke.Time || smoke.Time + THICK_SMOKE_DURATION < time)
                {
                    return false;
                }
            }

            // Only get eye level of pos1, as it's enough to check if one position's eyes can see the other's feet
            pos1 = GeometryHelper.GetEyeLevelVector(pos1, isDucking);

            var closestPointToSmokeOnTrajectory = GeometryHelper.GetClosestPointOnLineSegment(pos1, pos2, smoke.SmokeCenter());

            var dist = (smoke.SmokeCenter() - closestPointToSmokeOnTrajectory).Length();

            return dist <= SMOKE_RADIUS;
        }

        /// <summary>
        /// Indicates whether a player is looking at a smoke. Not taking into account map geometry.
        /// </summary>
        /// <param name="smoke"></param>
        /// <param name="pos">Th</param>
        /// <param name="view"></param>
        /// <param name="time">If null, assumes that the smoke is still fully active at the time.</param>
        /// <param name="isDucking"></param>
        /// <returns></returns>
        public static bool PlayerAimsAtSmoke(this Smoke smoke, Vector3 pos, Vector2 view, int? time, bool isDucking = false)
        {
            if (time != null)
            {
                if (time < smoke.Time || smoke.Time + THICK_SMOKE_DURATION < time)
                {
                    return false;
                }
            }

            var posEyeLevel = GeometryHelper.GetEyeLevelVector(pos, isDucking);

            var playerIsInSmoke = smoke.CoversPosition(posEyeLevel);
            if (playerIsInSmoke)
            {
                return true;
            }

            var closestPointToSmokeOnLineOfSight = GeometryHelper.GetClosestPointOnRay(posEyeLevel, view, smoke.SmokeCenter());

            // If the player is not in the smoke, and the closest point to the smoke on the ray is not the player's position, then it must not be in front of him.
            var smokeIsInFrontOfPlayer = !playerIsInSmoke && (closestPointToSmokeOnLineOfSight - posEyeLevel).Length() > 0.01;
            if (!smokeIsInFrontOfPlayer)
            {
                return false;
            }

            var distBetweenSmokeAndLineOfSight = (smoke.SmokeCenter() - closestPointToSmokeOnLineOfSight).Length();
            return distBetweenSmokeAndLineOfSight <= SMOKE_RADIUS;
        }

        /// <summary>
        /// Whether a position is inside the smoke.
        /// </summary>
        /// <param name="smoke"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static bool CoversPosition(this Smoke smoke, Vector3 pos)
        {
            var dist = (smoke.SmokeCenter() - pos).Length();
            return dist <= SMOKE_RADIUS;

        }

        public static Vector3 SmokeCenter(this Smoke smoke)
        {
            return new Vector3(smoke.DetonationPos.X, smoke.DetonationPos.Y, smoke.DetonationPos.Z + SMOKE_RADIUS);
        }
    }
}
