using MatchEntities.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SituationDatabase
{
    /// <summary>
    /// Extensions for IGrenadeThrow interface.
    /// </summary>
    public static class IGrenadeThrowExtensions
    {
        /// <summary>
        /// Returns the time the grenade was thrown.
        /// </summary>
        /// <param name="grenade"></param>
        /// <returns></returns>
        public static int GetThrowTime(this IGrenadeThrow grenade)
        {
            var trajectory = JsonConvert.DeserializeObject<List<TrajectoryPoint>>(grenade.Trajectory);
            return trajectory.First().Time;
        }

        /// <summary>
        /// Returns the time the grenade detonated.
        /// </summary>
        /// <param name="grenade"></param>
        /// <returns></returns>
        public static int GetDetonationTime(this IGrenadeThrow grenade)
        {
            var trajectory = JsonConvert.DeserializeObject<List<TrajectoryPoint>>(grenade.Trajectory);
            return trajectory.Last().Time;
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
