using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SituationOperator.Helpers
{
    /// <summary>
    /// Helper for converting between ingame and real world units.
    /// </summary>
    public static class UnitConverter
    {
        /// <summary>
        /// For more info, see https://developer.valvesoftware.com/wiki/Dimensions#Map_Grid_Units:_quick_reference.
        /// </summary>
        private const double UNITS_PER_METER = 52.5;

        /// <summary>
        /// </summary>
        /// <param name="meters"></param>
        /// <returns></returns>
        public static double MetersToUnits(double meters)
        {
            return meters * UNITS_PER_METER;
        }

        /// <summary>
        /// </summary>
        /// <param name="units"></param>
        /// <returns></returns>
        public static double UnitsToMeters(double units)
        {
            return units / UNITS_PER_METER;
        }
    }
}
