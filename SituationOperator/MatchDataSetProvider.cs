using MatchEntities;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SituationOperator
{
    public interface IMatchDataSetProvider
    {
        /// <summary>
        /// Provides the MatchDataSet for the given matchId.
        /// </summary>
        /// <param name="matchId"></param>
        /// <returns></returns>
        Task<MatchDataSet> GetMatchAsync(long matchId);
    }

    /// <summary>
    /// Communicates with the redis cache that stores MatchDataSets
    /// </summary>
    public class MatchRedis : IMatchDataSetProvider
    {
        private readonly ILogger<MatchRedis> _logger;
        private IDatabase cache;

        public MatchRedis(ILogger<MatchRedis> logger, IConnectionMultiplexer connectionMultiplexer)
        {
            _logger = logger;
            cache = connectionMultiplexer.GetDatabase();
        }

        /// <summary>
        /// Attempts to load a MatchDataSet from redis.
        /// </summary>
        /// <param name="matchId"></param>
        /// <returns></returns>
        public async Task<MatchDataSet> GetMatchAsync(long matchId)
        {
            var key = matchId.ToString();
            _logger.LogInformation($"Attempting to load match with key {key} from redis.");
            var response = await cache.StringGetAsync(key).ConfigureAwait(false);
            var match = MatchDataSet.FromJson(response.ToString());

            _logger.LogInformation($"Succesfully loaded Match with key {key} from redis.");
            return match;
        }
    }

    public class MockRedis : IMatchDataSetProvider
    {
        public async Task<MatchDataSet> GetMatchAsync(long matchId)
        {
            return new MatchDataSet();
        }
    }
}
