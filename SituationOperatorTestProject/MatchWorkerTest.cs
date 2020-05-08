using Microsoft.VisualStudio.TestTools.UnitTesting;
using SituationOperator;
using SituationOperator.Communications;
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

        [DataRow("TestDemo_Valve1.json")]
        [DataTestMethod]
        public async Task AnalysisTest(string jsonMatchDataPath)
        {
            // ARRANGE
            var matchDataSet = TestHelper.GetTestMatchData(jsonMatchDataPath);

            var managerProvider = TestHelper.GetRealProvider();

            var matchWorker = new MatchWorker(
                TestHelper.GetMockLogger<MatchWorker>(),
                TestHelper.GetTestContext(),
                managerProvider
                );

            // RUN
            var result = await matchWorker.ExtractAndUploadSituationsAsync(matchDataSet);

            // ASSERT
            Assert.IsTrue(result.AttemptedManagers > 0);
            Assert.IsTrue(result.FailedManagers == 0);
        }
    }
}
