using EquipmentLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SituationOperator.Helpers
{
    /// <summary>
    /// Extensions to simplify usage for EquipmentInfo from EquipmentLib.
    /// </summary>
    public static class EquipmentInfoExtensions
    {
        private static readonly List<string> _fireArms = new List<string>
        {
            "Pistol",
            "Shotgun",
            "SubMachinegun",
            "Machinegun",
            "Rifle",
            "SniperRifle"
        };

        /// <summary>
        /// Whether this is a piece of equipment that shoots bullets.
        /// </summary>
        /// <param name="equipmentInfo"></param>
        /// <returns></returns>
        public static bool IsFireArm(this EquipmentInfo equipmentInfo)
        {
            if (_fireArms.Contains(equipmentInfo.WeaponClass))
            {
                return true;
            }
            return false;
        }
    }
}
