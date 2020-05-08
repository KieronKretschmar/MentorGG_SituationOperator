using MatchEntities;
using Microsoft.DotNet.PlatformAbstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using RabbitCommunicationLib.Interfaces;
using SituationDatabase;
using SituationOperator;
using SituationOperator.Communications;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SituationOperatorTestProject
{
    public static class TestHelper
    {
        public static readonly string TestDataFolderName = "TestData";
        public static DbContextOptions<SituationContext> TestDbContextOptions = new DbContextOptionsBuilder<SituationContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        #region TestData
        public static string GetTestFilePath(string fileName)
        {
            var path = Path.Combine(GetTestDataFolderPath(), fileName);
            if (path.EndsWith(".dem") && !File.Exists(path))
            {
                throw new FileNotFoundException(".dem not found. You need to unzip it in order to run tests, since the unzipped file is too large for the repo.");
            }
            return path;
        }

        public static string GetTestDataFolderPath()
        {
            string startupPath = ApplicationEnvironment.ApplicationBasePath;
            var pathItems = startupPath.Split(Path.DirectorySeparatorChar);
            var pos = pathItems.Reverse().ToList().FindIndex(x => string.Equals("bin", x));
            string projectPath = String.Join(Path.DirectorySeparatorChar.ToString(), pathItems.Take(pathItems.Length - pos - 1));
            return Path.Combine(projectPath, TestDataFolderName);
        }

        public static MatchDataSet GetTestMatchData(string jsonFileName)
        {
            var path = GetTestFilePath(jsonFileName);
            var json = File.ReadAllText(path);
            var match = MatchDataSet.FromJson(json);
            return match;
        }
        #endregion

        #region Database
        public static void CleanTestDatabase()
        {
            using (var context = new SituationContext(TestDbContextOptions))
            {
                context.Database.EnsureDeleted();
            }
        }

        public static SituationContext GetTestContext()
        {
            var context = new SituationContext(TestDbContextOptions);
            return context;
        }

        #endregion

        #region Mock dependencies
        public static ILogger<T> GetMockLogger<T>()
        {
            return new Mock<ILogger<T>>().Object;
        }

        /// <summary>
        /// Returns mocked IMatchDataSetProvider that returns the dataSet from the specified json file.
        /// </summary>
        /// <param name="jsonFilePath"></param>
        /// <returns></returns>
        public static IMatchDataSetProvider GetMockMatchDataProvider(string jsonFilePath)
        {
            var json = File.ReadAllText(jsonFilePath);
            var match = MatchDataSet.FromJson(json);

            var mock = new Mock<IMatchDataSetProvider>();
            mock.Setup(x => x.GetMatchAsync(It.IsAny<long>()))
                .Returns(Task.FromResult(match));

            return mock.Object;
        }

        public static IProducer<SituationOperatorResponseModel> GetMockProducer()
        {
            return new Mock<IProducer<SituationOperatorResponseModel>>().Object;
        }
        public static IMatchWorker GetMockMatchWorker()
        {
            return new Mock<IMatchWorker>().Object;
        }
        #endregion

        #region Managers
        public static ISituationManagerProvider GetRealProvider()
        {
            return new SituationManagerProvider(GetMockLogger<SituationManagerProvider>());
        }
        #endregion
    }
}
