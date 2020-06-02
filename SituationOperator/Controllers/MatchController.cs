using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SituationDatabase;
using SituationOperator.Models;
using static SituationOperator.Models.MatchSituationsModel;

namespace SituationOperator.Controllers
{
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/public/match")]
    [ApiController]
    public class MatchController : ControllerBase
    {
        private readonly SituationContext _context;
        private readonly ISituationManagerProvider _managerProvider;

        public MatchController(SituationContext context, ISituationManagerProvider managerProvider)
        {
            _context = context;
            _managerProvider = managerProvider;
        }

        /// <summary>
        /// Get all Situations from a particular match.
        /// </summary>
        /// <param name="matchId"></param>
        /// <returns></returns>
        [HttpGet("{matchId}/situations")]
        public async Task<ActionResult<MatchSituationsModel>> MatchSituationsAsync(long matchId)
        {
            var match = _context.Match.SingleOrDefault(x => x.MatchId == matchId);
            if (match == null)
            {
                return NotFound();
            }

            var model = new MatchSituationsModel(match);

            var managers = _managerProvider.GetManagers(Enums.SituationTypeCollection.ProductionAccessDefault);

            foreach (var manager in managers)
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

            return model;
        }
    }
}