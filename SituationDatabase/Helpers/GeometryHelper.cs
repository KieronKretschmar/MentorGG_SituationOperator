using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace SituationDatabase.Helpers
{
    public class GeometryHelper
    {
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
        /// Determines the angle between the viewDirection of an object or player and a the line to another position.
        /// </summary>
        /// <param name="position1">Position of the first object or player</param>
        /// <param name="view1">Viewdirection of the first object or player</param>
        /// <param name="position2">Position of the second object or player</param>
        /// <returns>Angle theta in [0,360] in degrees </returns>
        public static double AngleFromViewDirection(Vector3 position1, Vector2 view1, Vector3 position2)
        {
            // TODO: Add height to get eyelevel new Vector3(position1.X, position1.Y, position1.Z + (isDucking ? 46 : 64));
            // Determine vector v1 from player to object
            var positionToObject = position2 - position1;

            // Determine vector v2 from player in direction of his view
            // The view directions are stored as follows:
            // Yaw == PlayerViewX, and 
            // Pitch == 90 - PlayerViewY.
            // Since C# computes in radiants, we also convert the degrees to radiants
            var yaw = DegreeToRadian(view1.X);
            var pitch = DegreeToRadian(90 - view1.Y);

            // Compute v2 from yaw and pitch
            var v2X = Math.Cos(yaw) * Math.Cos(pitch);
            var v2Y = Math.Sin(yaw) * Math.Cos(pitch);
            var v2Z = Math.Sin(pitch);
            var v2 = new Vector3((float)v2X, (float)v2Y, (float)v2Z); ;


            // Determine the angle theta between v1 and v2, using 
            // cos(theta) = v1 DOT v2 / (Length(v1) * Length(v2))
            var theta = Math.Acos(Vector3.Dot(positionToObject, v2) / (positionToObject.Length() * v2.Length()));

            // return resulting angle in degrees
            return RadianToDegree(theta);
        }

        private static double DegreeToRadian(double angle)
        {
            return Math.PI * angle / 180.0;
        }

        private static double RadianToDegree(double angle)
        {
            return angle * (180.0 / Math.PI);
        }
    }
}
