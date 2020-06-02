using Microsoft.Extensions.Logging;
using SituationDatabase;
using SituationOperator.Enums;
using SituationOperator.SituationManagers;
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
        /// <summary>
        /// Returns the specified collection of ISituationManagers.
        /// </summary>
        /// <param name="collectionIdentifier"></param>
        /// <returns></returns>
        IEnumerable<ISituationManager> GetManagers(SituationTypeCollection collection);

        /// <summary>
        /// Returns the specified collection of ISinglePlayerSituationManagers.
        /// </summary>
        /// <param name="collectionIdentifier"></param>
        /// <returns></returns>
        IEnumerable<ISinglePlayerSituationManager> GetSinglePlayerManagers(SituationTypeCollection collection);
    }

    /// <summary>
    /// Provides collections of ISituationManagers.
    /// </summary>
    public class SituationManagerProvider : ISituationManagerProvider
    {
        private readonly ILogger<SituationManagerProvider> _logger;

        /// <summary>
        /// Complete list of all implemented SituationManagers
        /// </summary>
        private readonly IEnumerable<ISituationManager> _situationManagers;

        public SituationManagerProvider(
            ILogger<SituationManagerProvider> logger,
            IEnumerable<ISituationManager> situationManagers)
        {
            _logger = logger;

            _situationManagers = situationManagers;
        }

        /// <inheritdoc/>
        public IEnumerable<ISituationManager> GetManagers(SituationTypeCollection collectionIdentifier)
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

        /// <inheritdoc/>
        public IEnumerable<ISinglePlayerSituationManager> GetSinglePlayerManagers(SituationTypeCollection collectionIdentifier)
        {
            _logger.LogTrace($"GetManagers called with identifier [ {collectionIdentifier} ]");

            switch (collectionIdentifier)
            {
                case SituationTypeCollection.ProductionExtractionDefault:
                case SituationTypeCollection.ProductionAccessDefault:
                    return _situationManagers
                        .Where(x=> x is ISinglePlayerSituationManager)
                        .Select(x=> x as ISinglePlayerSituationManager);
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
