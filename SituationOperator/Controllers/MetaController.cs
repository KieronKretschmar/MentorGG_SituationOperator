using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SituationDatabase;
using SituationOperator.Enums;
using SituationOperator.Helpers;
using SituationOperator.Helpers.SubscriptionConfig;
using SituationOperator.Models;
using static SituationOperator.Models.MatchSituationsModel;

namespace SituationOperator.Controllers
{
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/public/meta")]
    [ApiController]
    public class MetaController : ControllerBase
    {
        private readonly ISituationManagerProvider _managerProvider;
    
        public MetaController(
            ISituationManagerProvider managerProvider)        
        {
            _managerProvider = managerProvider;
        }

        /// <summary>
        /// Gets all SituationTypes' metadata objects.
        /// </summary>
        /// <returns></returns>
        [HttpGet("situationtype-meta-data")]
        public async Task<ActionResult<MetaDataCollection>> MetaDataCollectionAsync()
        {
            var managers = _managerProvider.GetSinglePlayerManagers(SituationTypeCollection.ProductionAccessDefault);
            var metaDataList = managers.Select(x => new SituationTypeMetaData(x.SituationType, x.SkillDomain)).ToList();

            var model = new MetaDataCollection(metaDataList);
            return model;
        }
    }
}