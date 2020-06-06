using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SituationDatabase;
using SituationOperator.Enums;
using SituationOperator.Helpers.SubscriptionConfig;
using SituationOperator.Models;
using static SituationOperator.Models.MatchSituationsModel;

namespace SituationOperator.Controllers
{
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/public/situationtype")]
    [ApiController]
    public class SituationTypeController : ControllerBase
    {
        private readonly SituationContext _context;
        private readonly ISituationManagerProvider _managerProvider;
        private readonly ISubscriptionConfigProvider _subscriptionConfigLoader;

        /// <summary>
        /// Upper limit for the matchCount param.
        /// </summary>
        private const int MAX_MATCHCOUNT = 100;

        public SituationTypeController(
            SituationContext context, 
            ISituationManagerProvider managerProvider,
            ISubscriptionConfigProvider subscriptionConfigLoader)
        {
            _context = context;
            _managerProvider = managerProvider;
            _subscriptionConfigLoader = subscriptionConfigLoader;
        }

        /// <summary>
        /// Get the most recent situations of the specified type.
        /// </summary>
        /// <param name="situationType"></param>
        /// <param name="matchCount">Number of matches for which to return situations.</param>
        /// <returns></returns>
        [HttpGet("{situationType}")]
        public async Task<ActionResult<SituationDetailModel>> SituationSamplesAsync(SituationType situationType, int matchCount, SubscriptionType subscriptionType)
        {
            var config = _subscriptionConfigLoader.Config.SettingsFromSubscriptionType(subscriptionType);

            var manager = _managerProvider.GetManager(situationType);
            if(matchCount > MAX_MATCHCOUNT)
            {
                return BadRequest($"MatchCount must be lower than [ {MAX_MATCHCOUNT} ]. Requested [ {matchCount} ].");
            }

            List<ISituation> situations = new List<ISituation>();
            var matches = _context.Match
                .OrderByDescending(x=>x.MatchId)
                .Take(matchCount)
                .Select(x => new MatchInfo(x, config.FirstAndLastRoundsForSituations))
                .ToDictionary(x => x.MatchId, x => x);

            var situationCollection = await manager.LoadSituationCollectionAsync(matches.Keys.ToList());

            var model = new SituationDetailModel(matches, situationCollection);

            return model;
        }
    }
}