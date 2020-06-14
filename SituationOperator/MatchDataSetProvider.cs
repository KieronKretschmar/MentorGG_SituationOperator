using MatchEntities;
using Microsoft.Extensions.Logging;
using SituationOperator.Helpers;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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
    public class MatchDataSetProvider : IMatchDataSetProvider
    {
        private readonly ILogger<MatchDataSetProvider> _logger;
        private readonly IHttpClientFactory _clientFactory;
        private readonly bool skipRedis;
        private IDatabase cache;

        public MatchDataSetProvider(
            ILogger<MatchDataSetProvider> logger, 
            IConnectionMultiplexer connectionMultiplexer,
            IHttpClientFactory clientFactory
            )
        {
            _logger = logger;
            _clientFactory = clientFactory;
            skipRedis = connectionMultiplexer == null;
            if (!skipRedis)
            {
                cache = connectionMultiplexer.GetDatabase();
            }
            else
            {
                _logger.LogWarning("Configured MatchDataSetProvider to skip retrieval through Redis.");
            }
        }

        /// <summary>
        /// Attempts to load a MatchDataSet from redis or via HTTP from MatchRetriever as fallback.
        /// </summary>
        /// <param name="matchId"></param>
        /// <returns></returns>
        public async Task<MatchDataSet> GetMatchAsync(long matchId)
        {
            if (!skipRedis)
            {
                try
                {
                    var match = await GetMatchDataSetFromRedis(matchId);
                    return match;
                }
                catch (Exception e)
                {
                    _logger.LogWarning(e, $"Failed to load match [ {matchId} ] from redis. Falling back to MatchRetriever.");
                }
            }

            try
            {
                var match = await GetMatchDataSetFromMatchRetriever(matchId);
                return match;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Finally failed to load match [ {matchId} ].");
                throw;
            }
        }

        /// <summary>
        /// Attempts to load a MatchDataSet from redis.
        /// </summary>
        /// <param name="matchId"></param>
        /// <returns></returns>
        public async Task<MatchDataSet> GetMatchDataSetFromRedis(long matchId)
        {
            var key = matchId.ToString();
            _logger.LogInformation($"Attempting to load match with key {key} from redis.");

            var response = await cache.StringGetAsync(key).ConfigureAwait(false);
            var match = MatchDataSet.FromJson(response.ToString());

            _logger.LogInformation($"Succesfully loaded match with key {key} from redis.");
            return match;
        }

        /// <summary>
        /// Attempts to load a MatchDataSet from MatchRetriever via HTTP.
        /// </summary>
        /// <param name="matchId"></param>
        /// <returns></returns>
        public async Task<MatchDataSet> GetMatchDataSetFromMatchRetriever(long matchId)
        {
            _logger.LogInformation($"Attempting to load match [ {matchId} ] from matchretriever.");
            var client = _clientFactory.CreateClient(ConnectedServices.MatchRetriever);

            HttpRequestMessage message = new HttpRequestMessage(
                HttpMethod.Get,
                $"v1/public/match/{matchId}/matchdataset");

            var response = await client.SendAsync(message);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var match = MatchDataSet.FromJson(json);

            _logger.LogInformation($"Succesfully loaded match [ {matchId} ] from matchretriever.");
            return match;
        }
    }

    public class MockMatchDataSetProvider : IMatchDataSetProvider
    {
        public async Task<MatchDataSet> GetMatchAsync(long matchId)
        {
            return new MatchDataSet();
        }
    }
}
