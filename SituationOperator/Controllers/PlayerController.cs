﻿using System;
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
        /// <returns></returns>
        [HttpGet("{steamId}/situations")]
        public async Task<ActionResult<PlayerSituationsModel>> PlayerSituationsAsync(long steamId, [CsvModelBinder] List<long> matchIds)
        {
            var model = new PlayerSituationsModel();

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
        /// <returns></returns>
        [HttpGet("{steamId}/situations/{situationType}")]
        public async Task<ActionResult<SituationCollection>> SituationCollectionAsync(long steamId, SituationType situationType, [CsvModelBinder] List<long> matchIds)
        {
            var model = new PlayerSituationsModel();

            var manager = _managerProvider.GetSinglePlayerManager(situationType);

            if(manager == null)
            {
                return NotFound($"Manager for SituationType [ {situationType} ] not found.");
            }

            var situationCollection = await manager.GetSituationCollectionAsync(steamId, matchIds);         

            return situationCollection;
        }
    }
}