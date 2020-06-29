using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SituationDatabase;
using SituationOperator.Enums;
using SituationOperator.Helpers;
using SituationOperator.Helpers.SubscriptionConfig;
using SituationOperator.Models;
using static SituationOperator.Models.MatchSituationsModel;

namespace SituationOperator.Controllers
{
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/public/player")]
    [ApiController]
    public class PlayerController : ControllerBase
    {
        private readonly ILogger<PlayerController> _logger;
        private readonly SituationContext _context;
        private readonly ISituationManagerProvider _managerProvider;
        private readonly ISubscriptionConfigProvider _subscriptionConfigLoader;

        public PlayerController(
            ILogger<PlayerController> logger,
            SituationContext context, 
            ISituationManagerProvider managerProvider,
            ISubscriptionConfigProvider subscriptionConfigLoader)
        {
            _logger = logger;
            _context = context;
            _managerProvider = managerProvider;
            _subscriptionConfigLoader = subscriptionConfigLoader;
        }

        /// <summary>
        /// Get all Situations from a given player and the given matches.
        /// </summary>
        /// <returns></returns>
        [HttpGet("{steamId}/situations")]
        public async Task<ActionResult<PlayerSituationsModel>> PlayerSituationsAsync(long steamId, [CsvModelBinder] List<long> matchIds, SubscriptionType subscriptionType)
        {
            var model = new PlayerSituationsModel();
            model.Matches = _context.Match
                .Where(x => matchIds.Contains(x.MatchId))
                .Select(x=>new MatchInfo(x, subscriptionType))
                .ToDictionary(x => x.MatchId, x => x);

            var managers = _managerProvider.GetSinglePlayerManagers(Enums.SituationTypeCollection.ProductionAccessDefault);
            foreach (var manager in managers)
            {
                try
                {
                    var situationCollection = await manager.GetSituationCollectionAsync(steamId, matchIds);

                    switch (manager.SituationCategory)
                    {
                        case SituationDatabase.Enums.SituationCategory.Misplay:
                            model.Misplays[situationCollection.MetaData.SituationType] = situationCollection;
                            break;
                        case SituationDatabase.Enums.SituationCategory.Highlight:
                            model.Highlights[situationCollection.MetaData.SituationType] = situationCollection;
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception e)
                {
                    _logger.LogWarning(e, $"Error when loading SituationCollection of type [ {manager.SituationType} ].");
                }
            }

            return model;
        }

        /// <summary>
        /// Get all Situations from a given player and the given matches.
        /// </summary>
        /// <param name="steamId"></param>
        /// <param name="situationType"></param>
        /// <param name="matchIds"></param>
        /// <returns></returns>
        [HttpGet("{steamId}/situations/{situationType}")]
        public async Task<ActionResult<SituationDetailModel>> SituationCollectionAsync(long steamId, SituationType situationType, [CsvModelBinder] List<long> matchIds, SubscriptionType subscriptionType)
        {
            var manager = _managerProvider.GetSinglePlayerManager(situationType);

            if(manager == null)
            {
                return NotFound($"Manager for SituationType [ {situationType} ] not found.");
            }

            var matches = _context.Match
                .Where(x => matchIds.Contains(x.MatchId))
                .Select(x => new MatchInfo(x, subscriptionType))
                .ToDictionary(x => x.MatchId, x => x);

            var situationCollection = await manager.GetSituationCollectionAsync(steamId, matchIds);

            var model = new SituationDetailModel(matches, situationCollection);

            return model;
        }
    }
}