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
        Task WorkAsync(RedisLocalizationInstruction model);
    }

    /// <summary>
    /// Handles instructions to analyze match.
    /// </summary>
    public class MessageProcessor : IMessageProcessor
    {
        private readonly ILogger<MessageProcessor> _logger;
        private readonly SituationContext _context;
        private readonly IMatchDataSetProvider _dataSetProvider;
        private readonly IProducer<SituationOperatorResponseModel> _producer;

        public MessageProcessor(
            ILogger<MessageProcessor> logger,
            SituationContext context,
            IMatchDataSetProvider dataSetProvider,
            IProducer<SituationOperatorResponseModel> producer
            )
        {
            _logger = logger;
            _context = context;
            _dataSetProvider = dataSetProvider;
            _producer = producer;
        }

        /// <summary>
        /// Instructs work on message and handles errors and outgoing communication.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task WorkAsync(RedisLocalizationInstruction model)
        {

        }
    }
}
