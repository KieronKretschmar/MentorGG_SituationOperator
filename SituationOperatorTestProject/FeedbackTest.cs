using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using RabbitCommunicationLib.Interfaces;
using RabbitCommunicationLib.TransferModels;
using RabbitMQ.Client.Events;
using SituationDatabase.Models;
using SituationOperator.Communications;
using SituationOperator.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SituationOperatorTestProject
{
    [TestClass]
    public class FeedbackTest
    {
        /// <summary>
        /// Tests whether RabbitConsumer calls the right methods and scoped dependencies are disposed.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task Test1()
        {
            // ARRANGE
            // Setup controller
            var context = TestHelper.GetInMemoryContext();
            var controller = new FeedbackController(
                TestHelper.GetMockLogger<FeedbackController>(),
                context);

            // Setup data
            var feedback1 = new UserFeedback(1, SituationDatabase.Enums.SituationType.Clutch, 1, 1, false, "feedback1");
            var feedback2 = new UserFeedback(1, SituationDatabase.Enums.SituationType.CollateralKill, 1, 1, true, "feedback2");
            var feedback1Version2 = new UserFeedback(1, SituationDatabase.Enums.SituationType.Clutch, 1, 1, false, "feedback1version2");

            // ACT
            await PostFeedback(controller, feedback1);
            await PostFeedback(controller, feedback2);
            await PostFeedback(controller, feedback1Version2);

            // ASSERT
            var feedbackModel = await controller.GetFeedbackAsync(1);
            Assert.AreEqual(2, feedbackModel.Value.UserFeedbacks.Count());
            var feedback1Version2FromDb = feedbackModel.Value.UserFeedbacks.Single(x => x.SituationType == SituationDatabase.Enums.SituationType.Clutch);
            Assert.AreEqual(JsonConvert.SerializeObject(feedback1Version2), JsonConvert.SerializeObject(feedback1Version2FromDb));
        }

        public async Task PostFeedback(FeedbackController controller, UserFeedback feedback)
        {
            await controller.PostFeedbackAsync(
                feedback.MatchId,
                feedback.SituationType,
                feedback.SituationId,
                feedback.SteamId,
                feedback.IsPositive,
                feedback.Comment);
        }
    }
}
