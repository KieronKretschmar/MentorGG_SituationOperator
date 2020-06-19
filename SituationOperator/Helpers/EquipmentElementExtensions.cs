using MatchEntities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SituationOperator.Helpers
{
    public static class EquipmentElementExtensions
    {
        public static readonly List<EquipmentElement> Grenades = new List<EquipmentElement>
        {
            EquipmentElement.Flash,
            EquipmentElement.Smoke,
            EquipmentElement.Molotov,
            EquipmentElement.Incendiary,
            EquipmentElement.HE,
            EquipmentElement.Decoy
        };

        public static readonly List<EquipmentElement> SniperRifles = new List<EquipmentElement>
        {
            EquipmentElement.Scout,
            EquipmentElement.AWP,
            EquipmentElement.Scar20,
            EquipmentElement.G3SG1
        };

        public static bool IsGrenade(this EquipmentElement element)
        {
            return Grenades.Contains(element);
        }

        public static bool IsGun(this EquipmentElement element)
        {
            return 1 <= (short) element && (short) element < 400;
        }

        public static bool IsRifle(this EquipmentElement element)
        {
            return 301 <= (short)element && (short)element < 400;
        }

        public static bool IsSniperRifle(this EquipmentElement element)
        {
            return SniperRifles.Contains(element);
        }
    }
}
