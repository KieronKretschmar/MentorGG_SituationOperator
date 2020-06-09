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

        /// <summary>
        /// Get EquipmentInfo of a given element. For more info see EquipmentLib.
        /// </summary>
        /// <param name="matchStats">Used for resolving Source and MatchDate</param>
        /// <returns></returns>
        Dictionary<EquipmentElement, EquipmentInfo> GetEquipmentDict(MatchStats matchStats);


        /// <summary>
        /// Get EquipmentInfo of a given element. For more info see EquipmentLib.
        /// </summary>
        /// <param name="equipmentElement"></param>
        /// <param name="matchStats">Used for resolving Source and MatchDate</param>
        /// <returns></returns>
        EquipmentInfo GetEquipmentInfo(EquipmentElement equipmentElement, MatchStats matchStats);
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

        /// <inheritdoc/>
        public EquipmentInfo GetEquipmentInfo(EquipmentElement equipmentElement, MatchStats matchStats)
        {
            var equipmentLibSource = (EquipmentLib.Enums.Source)matchStats.Source;
            EquipmentInfo equipmentInfo;
            if(!_equipmentProvider.GetEquipmentDict(equipmentLibSource, matchStats.MatchDate).TryGetValue((short)equipmentElement, out equipmentInfo))
            {
                return null;
            }
            return equipmentInfo;
        }

        /// <inheritdoc/>
        public Dictionary<EquipmentElement,EquipmentInfo> GetEquipmentDict(MatchStats matchStats)
        {
            var equipmentLibSource = (EquipmentLib.Enums.Source)matchStats.Source;
            return _equipmentProvider.GetEquipmentDict(equipmentLibSource, matchStats.MatchDate)
                .ToDictionary(x => (EquipmentElement)x.Key, x => x.Value);
        }
    }
}
