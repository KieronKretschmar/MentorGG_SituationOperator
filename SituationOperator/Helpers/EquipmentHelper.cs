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
        /// <param name="matchStats">Used for resolving Source and MatchDate</param>
        /// <returns></returns>
        Dictionary<EquipmentElement, EquipmentInfo> GetEquipmentDict(Source source, DateTime matchDate);

        /// <summary>
        /// Get EquipmentInfo of a given element. For more info see EquipmentLib.
        /// </summary>
        /// <param name="equipmentElement"></param>
        /// <param name="matchStats">Used for resolving Source and MatchDate</param>
        /// <returns></returns>
        EquipmentInfo GetEquipmentInfo(EquipmentElement equipmentElement, MatchStats matchStats);

        /// <summary>
        /// Get EquipmentInfo of a given element. For more info see EquipmentLib.
        /// </summary>
        /// <param name="equipmentElement"></param>
        /// <param name="matchStats">Used for resolving Source and MatchDate</param>
        /// <returns></returns>
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

        /// <inheritdoc/>
        public EquipmentInfo GetEquipmentInfo(EquipmentElement equipmentElement, MatchStats matchStats)
        {
            return GetEquipmentInfo(equipmentElement, matchStats.Source, matchStats.MatchDate);
        }

        /// <inheritdoc/>
        public EquipmentInfo GetEquipmentInfo(EquipmentElement equipmentElement, Source source, DateTime matchDate)
        {
            var equipmentLibSource = (EquipmentLib.Enums.Source)source;
            EquipmentInfo equipmentInfo;
            if (!_equipmentProvider.GetEquipmentDict(equipmentLibSource, matchDate).TryGetValue((short)equipmentElement, out equipmentInfo))
            {
                return null;
            }
            return equipmentInfo;
        }

        /// <inheritdoc/>
        public Dictionary<EquipmentElement,EquipmentInfo> GetEquipmentDict(MatchStats matchStats)
        {
            return GetEquipmentDict(matchStats.Source, matchStats.MatchDate);
        }

        /// <inheritdoc/>
        public Dictionary<EquipmentElement, EquipmentInfo> GetEquipmentDict(Source source, DateTime matchDate)
        {
            var equipmentLibSource = (EquipmentLib.Enums.Source)source;
            return _equipmentProvider.GetEquipmentDict(equipmentLibSource, matchDate)
                .ToDictionary(x => (EquipmentElement)x.Key, x => x.Value);
        }
    }
}
