using MatchEntities;
using Microsoft.Extensions.Logging;
using SituationDatabase;
using SituationDatabase.Models;
using SituationOperator.SituationManagers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SituationOperator
{
    /// <summary>
    /// Does work on MatchDataSets.
    /// </summary>
    public interface IMatchWorker
    {
        Task<ExtractionResult> ExtractAndUploadSituationsAsync(MatchDataSet matchData);
    }

    /// <summary>
    /// Does work on MatchDataSets.
    /// </summary>
    public class MatchWorker : IMatchWorker
    {
        private readonly ILogger<MatchWorker> _logger;
        private readonly SituationContext _context;
        private readonly ISituationManagerProvider _managerProvider;

        public MatchWorker(
            ILogger<MatchWorker> logger,
            SituationContext context,
            ISituationManagerProvider managerProvider
            )
        {
            _logger = logger;
            _context = context;
            _managerProvider = managerProvider;
        }

        /// <summary>
        /// Tries to extract situations from one matches data and uploads them to database.
        /// </summary>
        /// <param name="matchData">Data of the match.</param>
        /// <returns></returns>
        public async Task<ExtractionResult> ExtractAndUploadSituationsAsync(MatchDataSet matchData)
        {
            var managers = _managerProvider.GetManagers(Enums.SituationTypeCollection.ProductionExtractionDefault);
            var res = new ExtractionResult(managers);

            await RemoveMatchFromDatabaseAsync(matchData.MatchId);
            await UploadMetaDataAsync(matchData);

            // Iterate through all situationManagers to extract and upload to their respective tables
            foreach (var situationManager in managers)
            {
                try
                {
                    // Add situations to context
                    await situationManager.AnalyzeAndUploadAsync(matchData);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"Error when working on situations of type [ {situationManager.SituationType.ToString()} ] for match [ {matchData.MatchId} ]. Skipping this SituationManager.");
                    res.FailedManagers++;
                }
            }

            return res;
        }

        /// <summary>
        /// Uploads data from matches
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private async Task UploadMetaDataAsync(MatchDataSet data)
        {
            var match = new MatchEntity(data.MatchStats);
            _context.Match.Add(match);

            var rounds = data.RoundStatsList.Select(x => new RoundEntity(x));
            _context.Round.AddRange(rounds);

            var playerMatches = data.PlayerMatchStatsList.Select(x => new PlayerMatchEntity(x));
            _context.PlayerMatch.AddRange(playerMatches);

            var playerRounds = data.PlayerRoundStatsList.Select(x => new PlayerRoundEntity(x));
            _context.PlayerRound.AddRange(playerRounds);

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Removes all data from the given match.
        /// </summary>
        /// <param name="matchId"></param>
        /// <returns></returns>
        public async Task RemoveMatchFromDatabaseAsync(long matchId)
        {
            var match = _context.Match.SingleOrDefault(x=>x.MatchId == matchId);
            if(match != null)
            {
                // Remove Match entry, thereby removing all other entities through cascading ForeignKeys
                _context.Match.Remove(match);
                await _context.SaveChangesAsync();
            }
        }
    }

    /// <summary>
    /// Describes the outcome of an attempt to analyse and upload different situations in a match.
    /// </summary>
    public class ExtractionResult
    {
        public ExtractionResult(IEnumerable<ISituationManager> managers)
        {
            AttemptedManagers = managers.Count();
        }

        /// <summary>
        /// Number of SituationManagers that were attempted to be used for analysis.
        /// </summary>
        public int AttemptedManagers { get; set; }

        /// <summary>
        /// Number of SituationManagers that failed during analysis.
        /// </summary>
        public int FailedManagers { get; set; }
    }
}
