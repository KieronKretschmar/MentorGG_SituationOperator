using MatchEntities.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SituationDatabase
{
    /// <summary>
    /// Helper for dealing with trajectories encoded as strings.
    /// </summary>
    public static class TrajectoryHelper
    {

        /// <summary>
        /// Returns the time the grenade was thrown.
        /// </summary>
        /// <param name="trajectoryString"></param>
        /// <returns></returns>
        public static int GetDetonationTime(IGrenadeThrow grenade)
        {
            var trajectory = JsonConvert.DeserializeObject<List<TrajectoryPoint>>(grenade.Trajectory);
            return trajectory.Last().Time;
        }

        /// <summary>
        /// Returns the time the grenade was thrown.
        /// </summary>
        /// <param name="trajectoryString"></param>
        /// <returns></returns>
        public static int GetThrowTime(IGrenadeThrow grenade)
        {
            var trajectory = JsonConvert.DeserializeObject<List<TrajectoryPoint>>(grenade.Trajectory);
            return trajectory.First().Time;
        }

        /// <summary>
        /// Point in a trajectory of a grenade. This is a copy. Original lies in DemoFileWorker.DemoAnalyzer.
        /// </summary>
        public struct TrajectoryPoint
        {
            public int Time { get; set; }
            public double X { get; set; }
            public double Y { get; set; }
            public double Z { get; set; }

            public TrajectoryPoint(int time, double x, double y, double z)
            {
                Time = time;
                X = x;
                Y = y;
                Z = z;
            }

            public bool IsAlmostEqual(TrajectoryPoint other)
            {
                return Math.Abs(Time / 1000 - other.Time / 1000) < 0.5
                    && Math.Abs(X - other.X) < 0.5
                    && Math.Abs(Y - other.Y) < 0.5
                    && Math.Abs(Z - other.Z) < 0.5;
            }
        }
    }
}
