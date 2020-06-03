using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SituationDatabase;
using SituationOperator.Enums;
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
        /// <param name="nFirstAndLastRoundsPerHalf">        
        /// The number of rounds of the beginning and end of each half for which to allow situations.
        /// </param>
        /// <returns></returns>
        [HttpGet("{steamId}/situations")]
        public async Task<ActionResult<PlayerSituationsModel>> PlayerSituationsAsync(long steamId, [CsvModelBinder] List<long> matchIds, int? nFirstAndLastRoundsPerHalf = null)
        {
            var model = new PlayerSituationsModel();
            model.Matches = _context.Match
                .Where(x => matchIds.Contains(x.MatchId))
                .Select(x=>new MatchInfo(x, nFirstAndLastRoundsPerHalf))
                .ToList();

            var managers = _managerProvider.GetSinglePlayerManagers(Enums.SituationTypeCollection.ProductionAccessDefault);
            foreach (var manager in managers)
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

            return model;
        }

        /// <summary>
        /// Get all Situations from a given player and the given matches.
        /// </summary>
        /// <param name="steamId"></param>
        /// <param name="situationType"></param>
        /// <param name="matchIds"></param>
        /// <param name="nFirstAndLastRoundsPerHalf">
        /// The number of rounds of the beginning and end of each half for which to load situations. 
        /// A value of 1 means that misplays from the first and last round of each half are loaded.
        /// </param>
        /// <returns></returns>
        [HttpGet("{steamId}/situations/{situationType}")]
        public async Task<ActionResult<SituationDetailModel>> SituationCollectionAsync(long steamId, SituationType situationType, [CsvModelBinder] List<long> matchIds, int? nFirstAndLastRoundsPerHalf = null)
        {
            var manager = _managerProvider.GetSinglePlayerManager(situationType);

            if(manager == null)
            {
                return NotFound($"Manager for SituationType [ {situationType} ] not found.");
            }

            var matches = _context.Match
                .Where(x => matchIds.Contains(x.MatchId))
                .Select(x=>new MatchInfo(x, nFirstAndLastRoundsPerHalf))
                .ToList();

            var situationCollection = await manager.GetSituationCollectionAsync(steamId, matchIds);

            var model = new SituationDetailModel(matches, situationCollection);

            return model;
        }
    }
}