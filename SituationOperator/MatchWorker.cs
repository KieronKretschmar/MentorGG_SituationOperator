using MatchEntities;
using Microsoft.Extensions.Logging;
using SituationDatabase;
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
        Task ExtractAndUploadSituations(MatchDataSet matchData);
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
        public async Task ExtractAndUploadSituations(MatchDataSet matchData)
        {
            // Iterate through all situationManagers to extract and upload to their respective tables
            foreach (var situationManager in _managerProvider.GetManagers(Enums.SituationTypeCollection.ProductionExtractionDefault))
            {
                try
                {
                    // Extract situations from data
                    var situations = await situationManager.Detector.ExtractSituations(matchData);

                    var dbTable = situationManager.TableSelector(_context);

                    // Ensure no situations of this match from previous runs are in the table
                    var existingEntries = dbTable.Where(x => x.MatchId == matchData.MatchStats.MatchId);
                    dbTable.RemoveRange(existingEntries);

                    // Add situations to context
                    dbTable.AddRange(situations);

                    // Save changes to database
                    await _context.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"Error when working on situations of type [ {situationManager.SituationType.ToString()} ] for match [ {matchData.MatchStats.MatchId} ]. Skipping this SituationManager.");
                }
            }
        }
    }
}
