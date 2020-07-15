using SituationDatabase;
using SituationOperator.SituationManagers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SituationOperatorTestProject
{
    public static class SituationManagerHelper
    {
        public static IEnumerable<ISituationManager> GetSituationManagers(SituationContext context)
        {
            var serviceProviderHelper = new MockServiceProviderHelper();
            serviceProviderHelper.AddHelperServices();
            var serviceProvider = serviceProviderHelper.ServiceProviderMock.Object;

            var managers = new List<ISituationManager>();

            // Add managers
            // Code could possibly be simplified by requiring a uniform constructor for all ISituationManagers.

            #region Highlights - Singleplayer
            managers.Add(
                new EffectiveHeGrenadeManager(
                    serviceProvider,
                    TestHelper.GetMockLogger<EffectiveHeGrenadeManager>(),
                    context)
                );

            managers.Add(
                new KillWithOwnFlashAssistManager(
                    TestHelper.GetMockLogger<KillWithOwnFlashAssistManager>(),
                    context)
                );

            managers.Add(
                new ClutchManager(
                    serviceProvider,
                    TestHelper.GetMockLogger<ClutchManager>(),
                    context)
                );

            managers.Add(
                new HighImpactRoundManager(
                    serviceProvider,
                    TestHelper.GetMockLogger<HighImpactRoundManager>(),
                    context)
                );

            managers.Add(
                new MultiKillManager(
                    serviceProvider,
                    TestHelper.GetMockLogger<MultiKillManager>(),
                    context)
                );

            managers.Add(
                new TradeKillManager(
                    serviceProvider,
                    TestHelper.GetMockLogger<TradeKillManager>(),
                    context)
                );

            managers.Add(
                new KillThroughSmokeManager(
                    serviceProvider,
                    TestHelper.GetMockLogger<KillThroughSmokeManager>(),
                    context)
                );

            managers.Add(
                new WallBangKillManager(
                    serviceProvider,
                    TestHelper.GetMockLogger<WallBangKillManager>(),
                    context)
                );

            managers.Add(
                new CollateralKillManager(
                    serviceProvider,
                    TestHelper.GetMockLogger<CollateralKillManager>(),
                    context)
                );

            managers.Add(
                new FlashAssistManager(
                    serviceProvider,
                    TestHelper.GetMockLogger<FlashAssistManager>(),
                    context)
                );
            #endregion

            #region Misplays - Singleplayer
            managers.Add(
                new SmokeFailManager(
                    serviceProvider,
                    TestHelper.GetMockLogger<SmokeFailManager>(),
                    context)
                );

            managers.Add(
                new DeathInducedBombDropManager(
                    serviceProvider,
                    TestHelper.GetMockLogger<DeathInducedBombDropManager>(),
                    context)
                );

            managers.Add(
                new SelfFlashManager(
                    serviceProvider,
                    TestHelper.GetMockLogger<SelfFlashManager>(),
                    context)
                );

            managers.Add(
                new TeamFlashManager(
                    serviceProvider,
                    TestHelper.GetMockLogger<TeamFlashManager>(),
                    context)
                );

            managers.Add(
                new RifleFiredWhileMovingManager(
                    serviceProvider,
                    TestHelper.GetMockLogger<RifleFiredWhileMovingManager>(),
                    context)
                );

            managers.Add(
                new UnnecessaryReloadManager(
                    serviceProvider,
                    TestHelper.GetMockLogger<UnnecessaryReloadManager>(),
                    context)
                );

            managers.Add(
                new PushBeforeSmokeDetonatedManager(
                    serviceProvider,
                    TestHelper.GetMockLogger<PushBeforeSmokeDetonatedManager>(),
                    context)
                );

            managers.Add(
                new BombDropAtSpawnManager(
                    serviceProvider,
                    TestHelper.GetMockLogger<BombDropAtSpawnManager>(),
                    context)
                );

            managers.Add(
                new HasNotBoughtDefuseKitManager(
                    serviceProvider,
                    TestHelper.GetMockLogger<HasNotBoughtDefuseKitManager>(),
                    context)
                );

            managers.Add(
                new MissedTradeKillManager(
                    serviceProvider,
                    TestHelper.GetMockLogger<MissedTradeKillManager>(),
                    context)
                );
            #endregion

            return managers;
        }
    }
}
