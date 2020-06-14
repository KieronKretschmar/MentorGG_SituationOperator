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

        public static bool IsGrenade(this EquipmentElement element)
        {
            return Grenades.Contains(element);
        }
    }
}
