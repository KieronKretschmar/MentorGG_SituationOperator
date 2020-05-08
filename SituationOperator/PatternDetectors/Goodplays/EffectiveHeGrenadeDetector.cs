using MatchEntities;
using Microsoft.Extensions.Logging;
using SituationDatabase;
using SituationDatabase.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZoneReader;

namespace SituationOperator.PatternDetectors.Goodplays
{
    /// <summary>
    /// Detects effective HE grenades.
    /// </summary>
    public class EffectiveHeGrenadeDetector : IPatternDetector<EffectiveHeGrenade>
    {
        private readonly ILogger<EffectiveHeGrenadeDetector> _logger;

        /// <inheritdoc/>
        public EffectiveHeGrenadeDetector(
            ILogger<EffectiveHeGrenadeDetector> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="matchData">Data of the match in which to look for situations for all players.</param>
        /// <returns></returns>
        public async Task<IEnumerable<EffectiveHeGrenade>> ExtractSituations(MatchDataSet matchData)
        {
            throw new NotImplementedException();
        }
    }
}
