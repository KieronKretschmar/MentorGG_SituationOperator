using MatchEntities;
using Microsoft.Extensions.Logging;
using RabbitCommunicationLib.Enums;
using RabbitCommunicationLib.Interfaces;
using RabbitCommunicationLib.TransferModels;
using SituationDatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SituationOperator.Communications
{
    /// <summary>
    /// Handles instructions to analyze match.
    /// </summary>
    public interface IMessageProcessor
    {
        Task ProcessMessage(SituationExtractionInstruction model);
    }

    /// <summary>
    /// Handles instructions to analyze match.
    /// </summary>
    public class MessageProcessor : IMessageProcessor
    {
        private readonly ILogger<MessageProcessor> _logger;
        private readonly IMatchDataSetProvider _matchDataProvider;
        private readonly IProducer<SituationExtractionReport> _producer;
        private readonly IMatchWorker _matchWorker;

        public MessageProcessor(
            ILogger<MessageProcessor> logger,
            IMatchDataSetProvider matchDataProvider,
            IProducer<SituationExtractionReport> producer,
            IMatchWorker matchWorker
            )
        {
            _logger = logger;
            _matchDataProvider = matchDataProvider;
            _producer = producer;
            _matchWorker = matchWorker;
        }

        /// <summary>
        /// Loads data from redis and instructs work on it. Also takes care of error handling and outgoing communication.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task ProcessMessage(SituationExtractionInstruction model)
        {
            var response = new SituationExtractionReport(model.MatchId);

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
                    response.Block = DemoAnalysisBlock.MatchDataSetAccess;
                    _producer.PublishMessage(response);
                    return;
                }

                // Extract and upload situations
                await _matchWorker.ExtractAndUploadSituationsAsync(matchData);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Unknown exception when extracting and uploading situations for message [ {model.ToJson()} ].");
                response.Block = DemoAnalysisBlock.UnknownSituationOperator;
            }
            finally
            {
                _producer.PublishMessage(response);
            }
        }
    }
}
