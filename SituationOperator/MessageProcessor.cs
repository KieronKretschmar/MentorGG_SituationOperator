using MatchEntities;
using Microsoft.Extensions.Logging;
using RabbitCommunicationLib.Interfaces;
using RabbitCommunicationLib.TransferModels;
using SituationDatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SituationOperator
{
    /// <summary>
    /// Handles instructions to analyze match.
    /// </summary>
    public interface IMessageProcessor
    {
        Task ProcessMessage(RedisLocalizationInstruction model);
    }

    /// <summary>
    /// Handles instructions to analyze match.
    /// </summary>
    public class MessageProcessor : IMessageProcessor
    {
        private readonly ILogger<MessageProcessor> _logger;
        private readonly SituationContext _context;
        private readonly IMatchDataSetProvider _matchDataProvider;
        private readonly IProducer<SituationOperatorResponseModel> _producer;
        private readonly ISituationManagerProvider _managerProvider;

        public MessageProcessor(
            ILogger<MessageProcessor> logger,
            SituationContext context,
            IMatchDataSetProvider matchDataProvider,
            IProducer<SituationOperatorResponseModel> producer,
            ISituationManagerProvider managerProvider
            )
        {
            _logger = logger;
            _context = context;
            _matchDataProvider = matchDataProvider;
            _producer = producer;
            _managerProvider = managerProvider;
        }

        /// <summary>
        /// Loads data from redis and instructs work on it. Also takes care of error handling and outgoing communication.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task ProcessMessage(RedisLocalizationInstruction model)
        {
            var response = new SituationOperatorResponseModel();
            response.MatchId = model.MatchId;

            try
            {
                // Get MatchDataSet
                MatchDataSet matchData;
                try
                {
                    matchData = await _matchDataProvider.GetMatchAsync(model.MatchId);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"Error on accessing MatchDataSet for match [ {model.MatchId} ].");
                    response.Status = SituationOperatorResult.RedisError;
                    _producer.PublishMessage(response);
                    return;
                }

                // Extract and upload situations
                await ExtractAndUploadSituations(matchData);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Unknown exception when extracting and uploading situations for message [ {model.ToJson()} ].");
                response.Status = SituationOperatorResult.UnknownError;
            }
            finally
            {
                _producer.PublishMessage(response);
            }
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
