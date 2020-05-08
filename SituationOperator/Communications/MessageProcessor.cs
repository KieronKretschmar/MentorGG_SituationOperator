using MatchEntities;
using Microsoft.Extensions.Logging;
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
        Task ProcessMessage(RedisLocalizationInstruction model);
    }

    /// <summary>
    /// Handles instructions to analyze match.
    /// </summary>
    public class MessageProcessor : IMessageProcessor
    {
        private readonly ILogger<MessageProcessor> _logger;
        private readonly IMatchDataSetProvider _matchDataProvider;
        private readonly IProducer<SituationOperatorResponseModel> _producer;
        private readonly IMatchWorker _matchWorker;

        public MessageProcessor(
            ILogger<MessageProcessor> logger,
            IMatchDataSetProvider matchDataProvider,
            IProducer<SituationOperatorResponseModel> producer,
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
                await _matchWorker.ExtractAndUploadSituations(matchData);
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
    }
}
