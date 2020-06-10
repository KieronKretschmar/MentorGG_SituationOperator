using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SituationDatabase;
using SituationOperator.Enums;
using SituationOperator.Helpers.SubscriptionConfig;
using SituationOperator.Models;
using static SituationOperator.Models.MatchSituationsModel;

namespace SituationOperator.Controllers
{
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/public/match")]
    [ApiController]
    public class MatchController : ControllerBase
    {
        private readonly ILogger<MatchController> _logger;
        private readonly SituationContext _context;
        private readonly ISituationManagerProvider _managerProvider;
        private readonly ISubscriptionConfigProvider _subscriptionConfigLoader;

        public MatchController(
            ILogger<MatchController> logger, 
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
        /// Get all Situations from a particular match.
        /// </summary>
        /// <param name="matchId"></param>
        /// <param name="subscriptionType">
        /// The number of rounds of the beginning and end of each half for which to allow situations.
        /// </param>
        /// <returns></returns>
        [HttpGet("{matchId}/situations")]
        public async Task<ActionResult<MatchSituationsModel>> MatchSituationsAsync(long matchId, SubscriptionType subscriptionType)
        {
            var config = _subscriptionConfigLoader.Config.SettingsFromSubscriptionType(subscriptionType);

            var match = _context.Match.SingleOrDefault(x => x.MatchId == matchId);
            if (match == null)
            {
                return NotFound();
            }

            var model = new MatchSituationsModel(new MatchInfo(match, config.FirstAndLastRoundsForSituations));

            var managers = _managerProvider.GetManagers(Enums.SituationTypeCollection.ProductionAccessDefault);

            foreach (var manager in managers)
            {
                try
                {
                    var situationCollection = await manager.LoadSituationCollectionAsync(matchId);

                    switch (manager.SituationCategory)
                    {
                        case SituationDatabase.Enums.SituationCategory.Misplay:
                            model.Misplays.Add(situationCollection);
                            break;
                        case SituationDatabase.Enums.SituationCategory.Highlight:
                            model.Highlights.Add(situationCollection);
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
    }
}