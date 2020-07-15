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
    /// <summary>
    /// Read-Only controller that provides information about situations in the database.
    /// </summary>
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/trusted/distribution")]
    [ApiController]
    public class DistributionController : ControllerBase
    {
        private readonly ILogger<MatchController> _logger;
        private readonly SituationContext _context;
        private readonly ISituationManagerProvider _managerProvider;
        private readonly ISubscriptionConfigProvider _subscriptionConfigLoader;

        public DistributionController(
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
        /// Gets Info about the distribution of misplays
        /// </summary>
        /// <returns></returns>
        [HttpGet("by-rank")]
        public async Task<ActionResult<RankDistributionModel>> RankDistributionAsync()
        {
            var model = new RankDistributionModel();
            var managers = _managerProvider.GetManagers(SituationTypeCollection.ProductionAccessDefault);

            foreach (var manager in managers)
            {
                var rankTypeDistribution = await manager.GetDistribution();
                model.Data[manager.SituationType] = rankTypeDistribution;
            }

            return model;
        }
    }
}