using Honeylab.Analytics;
using Honeylab.Gameplay.Analytics;
using Honeylab.Utils.Analytics;
using Honeylab.Utils.App;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;


namespace Honeylab.Project
{
    public class ProjectAnalyticsInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<IAnalyticsService>()
                .FromMethod(CreateAnalyticsService)
                .AsSingle();

            Container.Bind<AnalyticsStartup>()
                .AsSingle();

            BindAppGlobalServices();
            BindAppStateTrackers();
            BindCommonPayloadServices();
        }


        private static IAnalyticsService CreateAnalyticsService(InjectContext ctx)
        {
            IAnalyticsService analyticsService = new AppMetricaAnalyticsService();

            DiContainer diContainer = ctx.Container;

            #if UNITY_EDITOR
            analyticsService = new LogAnalyticsServiceDecorator(analyticsService);
            #endif

            analyticsService = diContainer.Instantiate<CommonPayloadAnalyticsServiceDecorator>(new object[]
            {
                analyticsService
            });

            return analyticsService;
        }


        private void BindAppStateTrackers()
        {
            Container.Bind(typeof(AppStartTracker), typeof(IDisposable))
                .To<AppStartTracker>()
                .AsSingle();

            Container.Bind(typeof(GameStartTracker), typeof(IDisposable))
                .To<GameStartTracker>()
                .AsSingle();

            Container.BindInterfacesAndSelfTo<AppFocusStateTracker>()
                .AsSingle();

            Container.BindInterfacesAndSelfTo<HealthCheckTracker>()
                .AsSingle();

            Container.Bind<IEnumerable<IAppStateTracker>>()
                .FromMethod(ctx => ctx.Container.ResolveAll<IAppStateTracker>())
                .AsSingle();
        }


        private void BindCommonPayloadServices()
        {
            Container.BindInterfacesAndSelfTo<InterstitialsWatchCountReadService>()
                .AsSingle()
                .WithArguments(CommonAnalytics.PrefsKeys.InterstitialsWatchCount);

            Container.BindInterfacesAndSelfTo<InterstitialsWatchCountWriteService>()
                .AsSingle()
                .WithArguments(CommonAnalytics.PrefsKeys.InterstitialsWatchCount);

            Container.BindInterfacesAndSelfTo<RewardedVideosWatchCountService>()
                .AsSingle()
                .WithArguments(CommonAnalytics.PrefsKeys.RewardedVideosWatchCount);

            Container.Bind<CommonPayloadProvidersStorage>()
                .FromMethod(CreateCommonPayloadProvidersStorage)
                .AsSingle();
        }


        private void BindAppGlobalServices()
        {
            Container.BindInterfacesAndSelfTo<AppGlobalTimerService>()
                .AsSingle()
                .WithArguments(CommonAnalytics.PrefsKeys.SecondsPassedGlobal);

            Container.BindInterfacesAndSelfTo<AppInstallDateTimeService>()
                .AsSingle()
                .WithArguments(CommonAnalytics.PrefsKeys.AppInstallTimestamp);

            Container.BindInterfacesAndSelfTo<AppSessionService>()
                .AsSingle()
                .WithArguments(CommonAnalytics.PrefsKeys.SessionCount, 5.0);

            Container.BindInterfacesAndSelfTo<AppGlobalServicesStartup>()
                .AsSingle();
        }


        private CommonPayloadProvidersStorage CreateCommonPayloadProvidersStorage(InjectContext ctx)
        {
            CommonPayloadProvidersStorage storage = new();
            DiContainer diContainer = ctx.Container;

            storage.AddPayloadProvider(diContainer.Instantiate<MainCommonPayloadProvider>());

            return storage;
        }
    }
}
