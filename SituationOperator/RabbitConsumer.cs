using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitCommunicationLib.Consumer;
using RabbitCommunicationLib.Enums;
using RabbitCommunicationLib.Interfaces;
using RabbitCommunicationLib.TransferModels;
using RabbitMQ.Client.Events;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SituationOperator
{
    public class RabbitConsumer : FanOutConsumer<RedisLocalizationInstruction>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<RabbitConsumer> _logger;
        private const string _versionString = "1";

        public RabbitConsumer(
            IServiceProvider serviceProvider,
            IExchangeQueueConnection exchangeQueueConnection,
            ushort prefetchCount) : base(exchangeQueueConnection, prefetchCount)
        {
            _serviceProvider = serviceProvider;
            _logger = serviceProvider.GetRequiredService<ILogger<RabbitConsumer>>();
        }

        public override async Task<ConsumedMessageHandling> HandleMessageAsync(BasicDeliverEventArgs ea, RedisLocalizationInstruction model)
        {
            _logger.LogInformation($"Received message for Match [ {model.MatchId} ]: [ {model.ToJson()} ]");

            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var processor = scope.ServiceProvider.GetRequiredService<MessageProcessor>();
                    await processor.ProcessMessage(model);
                    return ConsumedMessageHandling.Done;
                }
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, $"Failed to handle message from DemoDownloaderReport queue. [ {model} ]");
                return ConsumedMessageHandling.Done;
            }
        }
    }
}
