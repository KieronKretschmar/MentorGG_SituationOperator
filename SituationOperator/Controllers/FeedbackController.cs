using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SituationDatabase;
using SituationDatabase.Enums;
using SituationDatabase.Models;
using SituationOperator.Enums;
using SituationOperator.Helpers;
using SituationOperator.Helpers.SubscriptionConfig;
using SituationOperator.Models;

namespace SituationOperator.Controllers
{
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/public/feedback")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        private readonly ILogger<FeedbackController> _logger;
        private readonly SituationContext _context;

        public FeedbackController(
            ILogger<FeedbackController> logger,
            SituationContext context)        
        {
            _logger = logger;
            _context = context;
        }

        /// <summary>
        /// Gets all SituationTypes' metadata objects.
        /// </summary>
        /// <returns></returns>
        [HttpGet("{steamId}")]
        public async Task<ActionResult<FeedbackModel>> GetFeedbackAsync(long steamId)
        {
            var feedbacks = await _context.UserFeedback
                .Where(x => x.SteamId == steamId)
                .ToListAsync();

            var model = new FeedbackModel(feedbacks);
            return model;
        }

        [HttpPost("{steamId}")]
        public async Task<ActionResult> PostFeedbackAsync(long matchId, SituationType situationType, long situationId, long steamId, bool isPositive, string comment)
        {
            var entity = await _context.UserFeedback.FindAsync(matchId, situationType, situationId, steamId);
            if(entity == null)
            {
                entity = new UserFeedback(matchId, situationType, situationId, steamId, isPositive, comment);
                _context.UserFeedback.Add(entity);
            }
            else
            {
                entity.IsPositive = isPositive;
                entity.Comment = comment;
            }

            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}