using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SituationOperator.Helpers
{
    /// <summary>
    /// Helper for converting between ingame and real world units.
    /// 
    /// For more info, see https://developer.valvesoftware.com/wiki/Dimensions#Map_Grid_Units:_quick_reference.
    /// </summary>
    public static class UnitConverter
    {
        /// <summary>
        /// 1 Meter ~ 525 units
        /// </summary>
        /// <param name="meters"></param>
        /// <returns></returns>
        public static double MetersToUnits(double meters)
        {
            return meters * 525;
        }
        /// <summary>
        /// 1 Meter ~ 525 units (see https://developer.valvesoftware.com/wiki/Dimensions#Map_Grid_Units:_quick_reference)
        /// </summary>
        /// <param name="units"></param>
        /// <returns></returns>
        public static double UnitsToMeters(double units)
        {
            return units / 525;
        }
    }
}
