using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SituationDatabase;
using SituationOperator;
using SituationOperator.Helpers;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SituationOperatorTestProject
{
    /// <summary>
    /// Collection of utility methods not meant for testing, for when you need a local database with data for debugging.
    /// </summary>
    [TestClass]
    public class UtilityMethods
    {
        /// <summary>
        /// Runs MatchWorker with partially mocked, but functional dependencies from TestHelper and real MatchDataSet from json and uploads the results to the real database.
        /// </summary>
        /// <param name="jsonFileName"></param>
        /// <returns></returns>
        [DataRow("TestDemo_Valve4.json")]
        [DataRow("TestDemo_Valve3.json")]
        [DataRow("TestDemo_Valve2.json")]
        [DataRow("TestDemo_Valve1.json")]
        [DataTestMethod]
        public async Task WorkJson(string jsonFileName)
        {
            // Make sure not to connect to real database with this testmethod. Use WorkFromMatchRetriever instead so MatchIds won't get mixed up.
            Assert.IsFalse(TestHelper.GetConnectionString().Contains(".mysql.database.azure.com"));

            var matchDataSet = TestHelper.GetTestMatchData(jsonFileName);
            SituationContext context = TestHelper.GetRealContext();
            var managerProvider = TestHelper.GetRealProvider(context);
            var matchWorker = new MatchWorker(
                TestHelper.GetMockLogger<MatchWorker>(),
                context,
                managerProvider
                );

            await matchWorker.ExtractAndUploadSituationsAsync(matchDataSet);
        }

        /// <summary>
        /// Runs MatchWorker with partially mocked, but functional dependencies from TestHelper and real MatchDataSet from json and uploads the results to the real database.
        /// </summary>
        /// <param name="jsonFileName"></param>
        /// <returns></returns>
        /// 
        [DataRow(110772)]
        [DataRow(91935)]
        [DataRow(82692)]
        [DataRow(82691)]
        [DataRow(82460)]
        [DataRow(76903)]
        [DataRow(76902)]
        [DataRow(75932)]
        [DataRow(75904)]
        [DataRow(49815)]
        [DataTestMethod]
        public async Task WorkFromMatchRetriever(long matchId)
        {
            var mockFactory = new Mock<IHttpClientFactory>();
            var client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:8081");
            client.DefaultRequestHeaders.Add("User-Agent", "SituationOperator_Test");

            mockFactory.Setup(_ => _.CreateClient(It.Is<string>(x=>x == ConnectedServices.MatchRetriever))).Returns(client);


            var provider = new MatchDataSetProvider(TestHelper.GetMockLogger<MatchDataSetProvider>(), null, mockFactory.Object);

            SituationContext context = TestHelper.GetRealContext();
            var managerProvider = TestHelper.GetRealProvider(context);
            var matchWorker = new MatchWorker(
                TestHelper.GetMockLogger<MatchWorker>(),
                context,
                managerProvider
                );

            var matchDataSet = await provider.GetMatchAsync(matchId);
            await matchWorker.ExtractAndUploadSituationsAsync(matchDataSet);
        }
    }
}
