using MatchEntities;
using Microsoft.DotNet.PlatformAbstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using RabbitCommunicationLib.Interfaces;
using RabbitCommunicationLib.TransferModels;
using SituationDatabase;
using SituationOperator;
using SituationOperator.Communications;
using SituationOperator.SituationManagers;
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
        public static string GetTestProjectPath()
        {
            string startupPath = ApplicationEnvironment.ApplicationBasePath;
            var pathItems = startupPath.Split(Path.DirectorySeparatorChar);
            var pos = pathItems.Reverse().ToList().FindIndex(x => string.Equals("bin", x));
            string projectPath = String.Join(Path.DirectorySeparatorChar.ToString(), pathItems.Take(pathItems.Length - pos - 1));
            return projectPath;
        }

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
            string projectPath = GetTestProjectPath();
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

        public static SituationContext GetInMemoryContext()
        {
            var context = new SituationContext(TestDbContextOptions);
            return context;
        }

        /// <summary>
        /// Provides a context for a mysql database, using the connectionstring in TestData/ConnectionString.txt.
        /// </summary>
        /// <returns></returns>
        public static SituationContext GetRealContext()
        {
            var connStringPath = GetTestFilePath("ConnectionString.txt");
            var connString = File.ReadAllText(connStringPath);
            var options = new DbContextOptionsBuilder<SituationContext>()
                .UseMySql(connString)
                .Options;
            var context = new SituationContext(options);
            return context;
        }


        #endregion

        #region Services
        /// <summary>
        /// Returns a ServiceProvider with helper services.
        /// </summary>
        /// <returns></returns>
        public static IServiceProvider GetServiceProvider()
        {
            // Setup MockServiceProviderHelper to return mockMessageProcessor
            var serviceProviderHelper = new MockServiceProviderHelper();



            return serviceProviderHelper.ServiceProviderMock.Object;
        }

        public static IEnumerable<ISituationManager> GetSituationManagers(SituationContext context)
        {
            var serviceProviderHelper = new MockServiceProviderHelper();
            serviceProviderHelper.AddHelperServices();
            var serviceProvider = serviceProviderHelper.ServiceProviderMock.Object;

            var managers = new List<ISituationManager>();

            // Add managers
            // Code could possibly be simplified by requiring a uniform constructor for all ISituationManagers.
            #region Misplays - Singleplayer
            managers.Add(
                new SmokeFailManager(
                    serviceProvider,
                    GetMockLogger<SmokeFailManager>(),
                    context)
                );

            managers.Add(
                new DeathInducedBombDropManager(
                    serviceProvider,
                    GetMockLogger<DeathInducedBombDropManager>(),
                    context)
                );

            managers.Add(
                new SelfFlashManager(
                    serviceProvider,
                    GetMockLogger<SelfFlashManager>(),
                    context)
                );

            managers.Add(
                new TeamFlashManager(
                    serviceProvider,
                    GetMockLogger<TeamFlashManager>(),
                    context)
                );

            managers.Add(
                new RifleFiredWhileMovingManager(
                    serviceProvider,
                    GetMockLogger<RifleFiredWhileMovingManager>(),
                    context)
                );

            managers.Add(
                new UnnecessaryReloadManager(
                    serviceProvider,
                    GetMockLogger<UnnecessaryReloadManager>(),
                    context)
                );

            managers.Add(
                new PushBeforeSmokeDetonatedManager(
                    serviceProvider,
                    GetMockLogger<PushBeforeSmokeDetonatedManager>(),
                    context)
                );

            managers.Add(
                new BombDropAtSpawnManager(
                    serviceProvider,
                    GetMockLogger<BombDropAtSpawnManager>(),
                    context)
                );

            managers.Add(
                new HasNotBoughtDefuseKitManager(
                    serviceProvider,
                    GetMockLogger<HasNotBoughtDefuseKitManager>(),
                    context)
                );
            #endregion

            #region Highlights - Singleplayer
            managers.Add(
                new EffectiveHeGrenadeManager(
                    serviceProvider,
                    GetMockLogger<EffectiveHeGrenadeManager>(),
                    context)
                );

            managers.Add(
                new KillWithOwnFlashAssistManager(
                    GetMockLogger<KillWithOwnFlashAssistManager>(),
                    context)
                );

            managers.Add(
                new ClutchManager(
                    serviceProvider,
                    GetMockLogger<ClutchManager>(),
                    context)
                );
            #endregion

            return managers;
        }

        public static ISituationManagerProvider GetRealProvider(SituationContext context)
        {
            return new SituationManagerProvider(
                GetMockLogger<SituationManagerProvider>(),
                GetSituationManagers(context)
                );
        }

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

        public static IProducer<SituationExtractionReport> GetMockProducer()
        {
            return new Mock<IProducer<SituationExtractionReport>>().Object;
        }

        public static IMatchWorker GetMockMatchWorker()
        {
            return new Mock<IMatchWorker>().Object;
        }
        #endregion

    }
}
