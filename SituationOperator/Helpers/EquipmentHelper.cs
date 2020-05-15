using EquipmentLib;
using MatchEntities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SituationOperator.Helpers
{
    /// <summary>
    /// Convenience wrapper for EquipmentLib.EquipmentProvider. 
    /// Primarily exists to remove need for casting enums when accessing the EquipmentLib's EquipmentProvider
    /// </summary>
    public interface IEquipmentHelper
    {
        EquipmentInfo GetEquipmentInfo(EquipmentElement equipmentElement, Source source, DateTime matchDate);
    }

    /// <summary>
    /// Convenience wrapper for EquipmentLib.EquipmentProvider
    /// </summary>
    public class EquipmentHelper : IEquipmentHelper
    {
        private readonly IEquipmentProvider _equipmentProvider;

        public EquipmentHelper(IEquipmentProvider equipmentProvider)
        {
            _equipmentProvider = equipmentProvider;
        }

        /// <summary>
        /// Get EquipmentInfo of a given element. For more info see EquipmentLib.
        /// </summary>
        /// <param name="equipmentElement"></param>
        /// <param name="source"></param>
        /// <param name="matchDate"></param>
        /// <returns></returns>
        public EquipmentInfo GetEquipmentInfo(MatchEntities.Enums.EquipmentElement equipmentElement, MatchEntities.Enums.Source source, DateTime matchDate)
        {
            var equipmentLibSource = (EquipmentLib.Enums.Source)source;
            // casts EquipmentElement enum and
            return _equipmentProvider.GetEquipmentDict(equipmentLibSource, matchDate)[(short)equipmentElement];
        }
    }
}
