﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SituationOperator.Models;

namespace SituationOperator.Controllers
{
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/public/match")]
    [ApiController]
    public class MatchController : ControllerBase
    {
        private readonly ISituationManagerProvider _managerProvider;

        public MatchController(ISituationManagerProvider managerProvider)
        {
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
            var model = new MatchSituationsModel();

            var managers = _managerProvider.GetManagers(Enums.SituationTypeCollection.ProductionAccessDefault);

            foreach (var manager in managers)
            {
                var situations = await manager.LoadSituationsAsync(matchId);

                switch (manager.SituationCategory)
                {
                    case SituationDatabase.Enums.SituationCategory.Misplay:
                        model.Misplays.Add(situations);
                        break;
                    case SituationDatabase.Enums.SituationCategory.Goodplay:
                        model.Highlights.Add(situations);
                        break;
                    default:
                        break;
                }
            }

            return model;
        }
    }
}