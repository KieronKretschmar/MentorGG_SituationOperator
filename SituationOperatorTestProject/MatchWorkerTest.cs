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
        [DataRow("TestDemo_Valve4.json", false)]
        [DataRow("TestDemo_Valve3.json", false)]
        [DataRow("TestDemo_Valve2.json", false)]
        [DataRow("TestDemo_Valve1.json", false)]
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

            // Assert that meta data was inserted into database
            Assert.IsTrue(context.Match.Count(x => x.MatchId == matchDataSet.MatchId) == 1);
            Assert.IsTrue(context.Round.Count(x => x.MatchId == matchDataSet.MatchId) > 0);
            Assert.IsTrue(context.PlayerMatch.Count(x => x.MatchId == matchDataSet.MatchId) > 0);
            Assert.IsTrue(context.PlayerRound.Count(x => x.MatchId == matchDataSet.MatchId) > 0);

            // Assert that at least some situation data was inserted to database
            Assert.IsTrue(context.EffectiveHeGrenade.Count() > 0);
        }

        /// <summary>
        /// Runs MatchWorker twice on match, and asserts that the only impact this has is that the Ids of situations change.
        /// 
        /// Notes:
        /// Assertions are made on one EffectiveHeGrenades only.
        /// </summary>
        /// <param name="jsonMatchDataPath"></param>
        /// <param name="connectionString">If provided, uses real database.</param>
        /// <returns></returns>
        [DataRow("TestDemo_Valve4.json")]
        [DataRow("TestDemo_Valve3.json")]
        [DataRow("TestDemo_Valve2.json")]
        [DataRow("TestDemo_Valve1.json")]
        [DataTestMethod]
        public async Task IdempotencyTest(string jsonMatchDataPath)
        {
            // ARRANGE
            var matchDataSet = TestHelper.GetTestMatchData(jsonMatchDataPath);

            // use Real or InMemory SituationContext
            SituationContext context = TestHelper.GetInMemoryContext();

            // Get managerProvider with some SituationManagers
            var managerProvider = TestHelper.GetRealProvider(context);

            var matchWorker = new MatchWorker(
                TestHelper.GetMockLogger<MatchWorker>(),
                context,
                managerProvider
                );

            // RUN
            // Analyze match
            var firstResult = await matchWorker.ExtractAndUploadSituationsAsync(matchDataSet);

            var effectiveHesAfterFirstRun = context.EffectiveHeGrenade.ToList();

            // Analyze match again
            var secondResult = await matchWorker.ExtractAndUploadSituationsAsync(matchDataSet);
            var effectiveHesAfterSecondRun = context.EffectiveHeGrenade.ToList();

            // ASSERT
            // Assert that at least some data was inserted to database
            var effectiveHes = context.EffectiveHeGrenade.ToList();
            Assert.IsTrue(effectiveHes.Count > 0);

            // Assert that database was not changed for effective HE data, except for the Id which is 
            // AutoGenerated and thus won't start at 1 on second iteration
            var zipped = effectiveHesAfterFirstRun.Zip(effectiveHesAfterSecondRun);
            Assert.IsTrue(zipped.All(x => EffectiveHeGrenadeIsEqualExceptForId(x.First, x.Second)));

            // Checks whether values except for Id are equal
            bool EffectiveHeGrenadeIsEqualExceptForId(EffectiveHeGrenade first, EffectiveHeGrenade second)
            {
                return first.MatchId == second.MatchId
                    && first.Round == second.Round
                    && first.TotalEnemyDamage == second.TotalEnemyDamage;
            }
        }

        /// <summary>
        /// Runs MatchWorker on match and calls RemoveMatchAsync, and asserts that metadata as well as a sample of Situations were removed from database.
        /// 
        /// Notes:
        /// Assertions are made on one EffectiveHeGrenades only.
        /// </summary>
        /// <param name="jsonMatchDataPath"></param>
        /// <param name="connectionString">If provided, uses real database.</param>
        /// <returns></returns>
        [DataRow("TestDemo_Valve4.json")]
        [DataRow("TestDemo_Valve3.json")]
        [DataRow("TestDemo_Valve2.json")]
        [DataRow("TestDemo_Valve1.json")]
        [DataTestMethod]
        public async Task RemoveMatchTest(string jsonMatchDataPath)
        {
            // ARRANGE
            var matchDataSet = TestHelper.GetTestMatchData(jsonMatchDataPath);

            // use Real or InMemory SituationContext
            SituationContext context = TestHelper.GetInMemoryContext();

            // Get managerProvider with some SituationManagers
            var managerProvider = TestHelper.GetRealProvider(context);

            var matchWorker = new MatchWorker(
                TestHelper.GetMockLogger<MatchWorker>(),
                context,
                managerProvider
                );

            // RUN
            // Analyze match
            await matchWorker.ExtractAndUploadSituationsAsync(matchDataSet);

            // Remove match
            await matchWorker.RemoveMatchFromDatabaseAsync(matchDataSet.MatchId);

            // ASSERT
            // Assert that no metadata is still in database
            Assert.IsFalse(context.Match.Any());
            Assert.IsFalse(context.Round.Any());
            Assert.IsFalse(context.PlayerMatch.Any());
            Assert.IsFalse(context.PlayerRound.Any());

            // Assert that no Situation is still in database, using effectiveHeGrenades as sample
            Assert.IsFalse(context.EffectiveHeGrenade.Any());
        }
    }
}
