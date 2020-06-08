using Microsoft.VisualStudio.TestTools.UnitTesting;
using SituationDatabase;
using SituationOperator;
using System;
using System.Collections.Generic;
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
    }
}
