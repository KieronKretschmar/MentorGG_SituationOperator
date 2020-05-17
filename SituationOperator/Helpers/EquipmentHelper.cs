using EquipmentLib;
using MatchEntities;
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
        EquipmentInfo GetEquipmentInfo(EquipmentElement equipmentElement, MatchDataSet data);
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
        /// <param name="data">Used for resolving Source and MatchDate</param>
        /// <returns></returns>
        public EquipmentInfo GetEquipmentInfo(MatchEntities.Enums.EquipmentElement equipmentElement, MatchDataSet data)
        {
            var equipmentLibSource = (EquipmentLib.Enums.Source)data.MatchStats.Source;
            // casts EquipmentElement enum and
            return _equipmentProvider.GetEquipmentDict(equipmentLibSource, data.MatchStats.MatchDate)[(short)equipmentElement];
        }
    }
}
