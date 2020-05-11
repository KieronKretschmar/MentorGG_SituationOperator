using Microsoft.VisualStudio.TestTools.UnitTesting;
using SituationOperator;
using SituationOperator.Communications;
using System.Linq;
using System.Threading.Tasks;

namespace SituationOperatorTestProject
{
    [TestClass]
    public class MatchWorkerTest
    {

        [TestCleanup]
        public void Cleanup()
        {
            TestHelper.CleanTestDatabase();
        }

        [DataRow("TestDemo_Valve4.json")]
        [DataRow("TestDemo_Valve3.json")]
        [DataRow("TestDemo_Valve2.json")]
        [DataRow("TestDemo_Valve1.json")]
        [DataTestMethod]
        public async Task AnalysisTest(string jsonMatchDataPath)
        {
            // ARRANGE
            var matchDataSet = TestHelper.GetTestMatchData(jsonMatchDataPath);

            var context = TestHelper.GetTestContext();
            var managerProvider = TestHelper.GetRealProvider(context);

            var matchWorker = new MatchWorker(
                TestHelper.GetMockLogger<MatchWorker>(),
                context,
                managerProvider
                );

            // RUN
            var result = await matchWorker.ExtractAndUploadSituationsAsync(matchDataSet);

            // ASSERT
            Assert.IsTrue(result.AttemptedManagers > 0);
            Assert.IsTrue(result.FailedManagers == 0);

            // Assert on a sample basis that data was inserted to database
            var effectiveHes = context.EffectiveHeGrenade.ToList();
            Assert.IsTrue(effectiveHes.Count > 0);
        }
    }
}
