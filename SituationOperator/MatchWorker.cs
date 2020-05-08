using MatchEntities;
using Microsoft.Extensions.Logging;
using SituationDatabase;
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

            // Iterate through all situationManagers to extract and upload to their respective tables
            foreach (var situationManager in managers)
            {
                try
                {
                    // Ensure no situations of this match from previous runs are in the table
                    await situationManager.ClearTableAsync(_context, matchData.MatchId);

                    // Add situations to context
                    await situationManager.AnalyzeAndUploadAsync(_context, matchData);

                    // Save changes to database
                    await _context.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"Error when working on situations of type [ {situationManager.SituationCategory.ToString()} ] for match [ {matchData.MatchId} ]. Skipping this SituationManager.");
                    res.FailedManagers++;
                }
            }

            return res;
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
