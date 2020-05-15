using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RabbitCommunicationLib.Interfaces;
using RabbitCommunicationLib.TransferModels;
using RabbitMQ.Client.Events;
using SituationOperator.Communications;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SituationOperatorTestProject
{
    [TestClass]
    public class RabbitConsumerTest
    {
        /// <summary>
        /// Tests whether RabbitConsumer calls the right methods and services are disposed.
        /// 
        /// Non working because it's impossible to mock IServiceProvider.GetRequiredService<>(), as Moq throws this error:
        /// Extension methods (here: ServiceProviderServiceExtensions.GetRequiredService) may not be used in setup / verification expressions.
        /// 
        /// Test is ignored but left here for reference.
        /// </summary>
        /// <returns></returns>
        [Ignore]
        [TestMethod]
        public async Task ConsumeMessageTest()
        {
            // ARRANGE
            // Setup mocked MessageProcessor
            var mockMessageProcessor = new Mock<IMessageProcessor>();
            // Setup mocked scoped ServiceProvider that returns mocked MessageProcessor
            var scopedServiceProvider = new Mock<IServiceProvider>();
            scopedServiceProvider.Setup(x => x.GetRequiredService<IMessageProcessor>())
                .Returns(mockMessageProcessor.Object);
            // mock scope that returns scopedServiceProvider
            var mockScope = new Mock<IServiceScope>();
            mockScope.Setup(x => x.ServiceProvider)
                .Returns(scopedServiceProvider.Object);
            // Setup serviceprovider that returns the mocked scope
            var mockServiceProvider = new Mock<IServiceProvider>();

            var mockExchangeQueueConnection = new Mock<IExchangeQueueConnection>();

            var consumer = new RabbitConsumer(TestHelper.GetMockLogger<RabbitConsumer>(), mockServiceProvider.Object, mockExchangeQueueConnection.Object, 0);

            // Setup message
            var matchId = 2;
            var redisKey = "myKey";
            var model = new RedisLocalizationInstruction
            {
                MatchId = matchId,
                ExpiryDate = DateTime.Now.AddDays(1),
                RedisKey = redisKey
            };
            var basicDeliverEventAgents = new BasicDeliverEventArgs();

            // ACT
            // Emulate Receival of message
            await consumer.HandleMessageAsync(basicDeliverEventAgents, model);

            // ASSERT
            // Assert that MessageProcessor.ProcessMessage was called with the given model
            mockMessageProcessor.Verify(x => x.ProcessMessage(It.Is<RedisLocalizationInstruction>(x => x.ToJson() == model.ToJson())), Times.Once);
            // Verify that the scope was disposed
            mockScope.Verify(x => x.Dispose(), Times.Once);

        }
    }
}
