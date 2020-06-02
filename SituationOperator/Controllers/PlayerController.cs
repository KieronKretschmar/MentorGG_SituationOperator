using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SituationDatabase;
using SituationOperator.Helpers;
using SituationOperator.Models;
using static SituationOperator.Models.MatchSituationsModel;

namespace SituationOperator.Controllers
{
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/public/player")]
    [ApiController]
    public class PlayerController : ControllerBase
    {
        private readonly SituationContext _context;
        private readonly ISituationManagerProvider _managerProvider;

        public PlayerController(SituationContext context, ISituationManagerProvider managerProvider)
        {
            _context = context;
            _managerProvider = managerProvider;
        }

        /// <summary>
        /// Get all Situations from a given player and the given matches.
        /// </summary>
        /// <returns></returns>
        [HttpGet("{steamId}/situations")]
        public async Task<ActionResult<PlayerSituationsModel>> PlayerSituationsAsync(long steamId, [CsvModelBinder] List<long> matchIds)
        {
            var model = new PlayerSituationsModel();

            var managers = _managerProvider.GetSinglePlayerManagers(Enums.SituationTypeCollection.ProductionAccessDefault);

            foreach (var manager in managers)
            {
                var situations = await manager.LoadSituationsAsync(steamId, matchIds);
                var situationCollection = new SituationCollection(manager.SituationType, situations);

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

            return model;
        }
    }
}