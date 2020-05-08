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
        IEnumerable<SituationManager> GetManagers(SituationTypeCollection collection);
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
        private readonly IEnumerable<SituationManager> SituationManagers;

        public SituationManagerProvider(
            ILogger<SituationManagerProvider> logger)
        {
            _logger = logger;

            SituationManagers = new List<SituationManager>
            {
            };
        }

        /// <summary>
        /// Returns the specified collection of ISituationManagers.
        /// </summary>
        /// <param name="collectionIdentifier"></param>
        /// <returns></returns>
        public IEnumerable<SituationManager> GetManagers(SituationTypeCollection collectionIdentifier)
        {
            _logger.LogTrace($"GetManagers called with identifier [ {collectionIdentifier} ]");

            switch (collectionIdentifier)
            {
                case SituationTypeCollection.ProductionExtractionDefault:
                case SituationTypeCollection.ProductionAccessDefault:
                    return SituationManagers;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
