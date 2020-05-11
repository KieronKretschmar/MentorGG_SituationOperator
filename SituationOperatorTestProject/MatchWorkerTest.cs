using Microsoft.VisualStudio.TestTools.UnitTesting;
using SituationDatabase;
using SituationDatabase.Models;
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

        /// <summary>
        /// Runs MatchWorker with partially mocked, but functional dependencies from TestHelper and real MatchDataSet from json. 
        /// Asserts that Situations are inserted to database.
        /// 
        /// Notes:
        /// Assertions are made on EffectiveHeGrenades only.
        /// Can also be used to insert data into real database.
        /// </summary>
        /// <param name="jsonMatchDataPath"></param>
        /// <param name="connectionString">If provided, uses real database.</param>
        /// <returns></returns>
        [DataRow("TestDemo_Valve4.json", true)]
        [DataRow("TestDemo_Valve3.json", true)]
        [DataRow("TestDemo_Valve2.json", true)]
        [DataRow("TestDemo_Valve1.json", true)]
        [DataTestMethod]
        public async Task AnalysisTest(string jsonMatchDataPath, bool useRealDatabase = false)
        {
            // ARRANGE
            var matchDataSet = TestHelper.GetTestMatchData(jsonMatchDataPath);

            // Use Real or InMemory SituationContext
            SituationContext context = useRealDatabase 
                ? TestHelper.GetRealContext()
                : TestHelper.GetInMemoryContext();

            // Get managerProvider with some SituationManagers
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
            Assert.IsTrue(result.FailedManagers < result.AttemptedManagers);

            // Assert on a sample basis that data was inserted to database
            var effectiveHes = context.EffectiveHeGrenade.ToList();
            Assert.IsTrue(effectiveHes.Count > 0);
        }
    }
}
