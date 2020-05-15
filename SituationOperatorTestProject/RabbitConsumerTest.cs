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
        /// Tests whether RabbitConsumer calls the right methods and scoped dependencies are disposed.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task ConsumeMessageTest()
        {
            // ARRANGE
            // Setup mocked MessageProcessor
            var mockMessageProcessor = new Mock<IMessageProcessor>();

            // Setup MockServiceProviderHelper to return mockMessageProcessor
            var serviceProviderHelper = new MockServiceProviderHelper();
            serviceProviderHelper.AddMockedService<IMessageProcessor>(mockMessageProcessor);

            // Create consumer
            var mockExchangeQueueConnection = new Mock<IExchangeQueueConnection>();
            var consumer = new RabbitConsumer(TestHelper.GetMockLogger<RabbitConsumer>(), serviceProviderHelper.ServiceProviderMock.Object, mockExchangeQueueConnection.Object, 0);

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
            serviceProviderHelper.ServiceScopeMock.Verify(x => x.Dispose(), Times.Once);

        }
    }
}
