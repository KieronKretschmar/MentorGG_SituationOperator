using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace SituationDatabase.Helpers
{
    public class GeometryHelper
    {
        #region Pure Geometry
        /// <summary>
        /// Returns a Vector3 on the line segment between A and B.
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <param name="P"></param>
        /// <returns></returns>
        public static Vector3 GetClosestPointOnLineSegment(Vector3 A, Vector3 B, Vector3 P)
        {
            Vector3 AP = P - A;       //Vector from A to P   
            Vector3 AB = B - A;       //Vector from A to B  

            float magnitudeAB = AB.LengthSquared();     //Magnitude of AB vector (it's length squared)     
            float ABAPproduct = Vector3.Dot(AP, AB);    //The DOT product of a_to_p and a_to_b     
            float distance = ABAPproduct / magnitudeAB; //The normalized "distance" from a to your closest point  

            if (distance < 0)     //Check if P projection is over vectorAB     
            {
                return A;

            }
            else if (distance > 1)
            {
                return B;
            }
            else
            {
                return A + AB * distance;
            }
        }

        /// <summary>
        /// Returns a Vector3 on the ray from A to B.
        /// </summary>
        /// <param name="A">Start of ray</param>
        /// <param name="B">Point on ray</param>
        /// <param name="P">Point for which to return closest point on ray</param>
        /// <returns></returns>
        public static Vector3 GetClosestPointOnRay(Vector3 A, Vector3 B, Vector3 P)
        {
            Vector3 AP = P - A;       //Vector from A to P   
            Vector3 AB = B - A;       //Vector from A to B  

            float magnitudeAB = AB.LengthSquared();     //Magnitude of AB vector (it's length squared)     
            float ABAPproduct = Vector3.Dot(AP, AB);    //The DOT product of a_to_p and a_to_b     
            float distance = ABAPproduct / magnitudeAB; //The normalized "distance" from a to your closest point  

            if (distance < 0)     //Check if P projection is over vectorAB     
            {
                return A;
            }
            else
            {
                return A + AB * distance;
            }
        }
        private static double DegreeToRadian(double angle)
        {
            return Math.PI * angle / 180.0;
        }

        private static double RadianToDegree(double angle)
        {
            return angle * (180.0 / Math.PI);
        }

        #endregion

        #region CS:GO related

        /// <summary>
        /// Returns a Vector3 on the ray from A in direction of view.
        /// </summary>
        /// <param name="A">Start of ray</param>
        /// <param name="view">Direction of ray, format as stored in database</param>
        /// <param name="P">Point for which to return closest point on ray</param>
        /// <returns></returns>
        public static Vector3 GetClosestPointOnRay(Vector3 A, Vector2 view, Vector3 P)
        {
            Vector3 pointOnRay = A + GetViewVectorFromView(view);
            return GetClosestPointOnRay(A, pointOnRay, P);
        }

        /// <summary>
        /// Returns a vector on eyelevel, assuming <paramref name="position"/> is the groundlevel position of a player like in the MatchDb.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="isDucking">If null, returns same position.</param>
        /// <returns></returns>
        public static Vector3 GetEyeLevelVector(Vector3 position, bool? isDucking = null)
        {
            return isDucking == null ? position : new Vector3(position.X, position.Y, position.Z + ((bool)isDucking ? 46 : 64));
        }

        /// <summary>
        /// Determines the angle between the viewDirection of an object or player and a the line to another position.
        /// </summary>
        /// <param name="playerPosition">Position of the first object or player</param>
        /// <param name="playerView">Viewdirection of the first object or player</param>
        /// <param name="position2">Position of the second object or player</param>
        /// <returns>Angle theta in [0,360] in degrees </returns>
        public static double AngleFromViewDirection(Vector3 playerPosition, Vector2 playerView, Vector3 position2, bool playerIsDucking = false)
        {
            // Determine vector v1 from player to object
            var positionToObject = position2 - GetEyeLevelVector(playerPosition, isDucking: playerIsDucking);

            // Determine vector from position1 in direction of view1
            var v2 = GetViewVectorFromView(playerView);

            // Determine the angle theta between v1 and v2, using 
            // cos(theta) = v1 DOT v2 / (Length(v1) * Length(v2))
            var theta = Math.Acos(Vector3.Dot(positionToObject, v2) / (positionToObject.Length() * v2.Length()));

            // return resulting angle in degrees
            return RadianToDegree(theta);
        }

        /// <summary>
        /// Determines a normalized Vector3 View vector with XYZ coordinates from a Vector2 as stored in the MatchDb.
        /// 
        /// The view directions are stored as follows:
        /// Yaw == PlayerViewX, and 
        /// Pitch == 90 - PlayerViewY.
        /// </summary>
        /// <param name="view">A Vector2 as stored in the MatchDb</param>
        /// <returns></returns>
        private static Vector3 GetViewVectorFromView(Vector2 view)
        {
            // Since C# computes in radiants, we also convert the degrees to radiants
            var yaw = DegreeToRadian(view.X);
            var pitch = DegreeToRadian(90 - view.Y);

            return GetViewVectorFromPitchAndYaw((float)pitch, (float)yaw);
        }

        /// <summary>
        /// Determines a normalized Vector3 from pitch and yaw.
        /// </summary>
        /// <param name="pitch"></param>
        /// <param name="yaw"></param>
        /// <returns></returns>
        private static Vector3 GetViewVectorFromPitchAndYaw(float pitch, float yaw)
        {
            // Compute v2 from yaw and pitch
            var v2X = Math.Cos(yaw) * Math.Cos(pitch);
            var v2Y = Math.Sin(yaw) * Math.Cos(pitch);
            var v2Z = Math.Sin(pitch);
            var v2 = new Vector3((float)v2X, (float)v2Y, (float)v2Z);

            return Vector3.Normalize(v2);
        }

        #endregion


    }
}
