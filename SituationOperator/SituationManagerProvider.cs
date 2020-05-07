using Microsoft.Extensions.Logging;
using SituationOperator.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SituationOperator
{
    /// <summary>
    /// Provides collections of ISituationManagers.
    /// </summary>
    public interface ISituationManagerProvider
    {
        IEnumerable<ISituationManager<ISituation>> GetManagers(SituationTypeCollection collection);
    }

    /// <summary>
    /// Provides collections of ISituationManagers.
    /// </summary>
    public class SituationManagerProvider : ISituationManagerProvider
    {
        private readonly ILogger<SituationManagerProvider> _logger;
        private readonly IEnumerable<ISituationManager<ISituation>> _situationManagers;

        public SituationManagerProvider(
            ILogger<SituationManagerProvider> logger,
            IEnumerable<ISituationManager<ISituation>> situationManagers)
        {
            _logger = logger;
            _situationManagers = situationManagers;
        }

        /// <summary>
        /// Returns the specified collection of ISituationManagers.
        /// </summary>
        /// <param name="collectionIdentifier"></param>
        /// <returns></returns>
        public IEnumerable<ISituationManager<ISituation>> GetManagers(SituationTypeCollection collectionIdentifier)
        {
            _logger.LogTrace($"GetManagers called with identifier [ {collectionIdentifier} ]");

            switch (collectionIdentifier)
            {
                case SituationTypeCollection.ProductionExtractionDefault:
                case SituationTypeCollection.ProductionAccessDefault:
                    return _situationManagers;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
