using Cysharp.Threading.Tasks;
using Honeylab.Analytics;
using Honeylab.Project;
using Honeylab.Project.Levels;
using Honeylab.Utils.Analytics;
using Honeylab.Utils.App;
using Honeylab.Utils.Configs;
using Honeylab.Utils.Extensions;
using Honeylab.Utils.Persistence;
using Honeylab.Utils.PushNotifications;
using System;
using System.Globalization;
using System.Threading;
using UnityEngine;
using Zenject;


namespace Honeylab.Startup
{
    public class ProjectStartup : IInitializable, IDisposable
    {
        private readonly CancellationTokenSource _disposeCts = new();
        private readonly ProjectConfig _config;
        private readonly PersistenceStartup _persistenceStartup;
        private readonly LoadingScreen _loadingScreen;
        private readonly RemoteConfigsService _remoteConfigsService;
        private readonly LevelsLoadService _levelsLoadService;
        private readonly AnalyticsStartup _analyticsStartup;
        private readonly AppGlobalServicesStartup _appGlobalServicesStartup;
        private readonly AppStartTracker _appStartTracker;
        private readonly IPushNotificationService _pushNotificationsService;


        public ProjectStartup(ProjectConfig config,
            PersistenceStartup persistenceStartup,
            LoadingScreen loadingScreen,
            RemoteConfigsService remoteConfigsService,
            LevelsLoadService levelsLoadService,
            AnalyticsStartup analyticsStartup,
            AppGlobalServicesStartup appGlobalServicesStartup,
            AppStartTracker appStartTracker,
            IPushNotificationService pushNotificationsService)
        {
            _config = config;
            _persistenceStartup = persistenceStartup;
            _loadingScreen = loadingScreen;
            _remoteConfigsService = remoteConfigsService;
            _levelsLoadService = levelsLoadService;
            _analyticsStartup = analyticsStartup;
            _appGlobalServicesStartup = appGlobalServicesStartup;
            _appStartTracker = appStartTracker;
            _pushNotificationsService = pushNotificationsService;
        }


        public void Initialize()
        {
            _appStartTracker.Run();
            _appGlobalServicesStartup.Run();
            InitializeAsync(_disposeCts.Token).Forget();
        }


        public void Dispose() => _disposeCts.CancelThenDispose();


        private async UniTaskVoid InitializeAsync(CancellationToken ct)
        {
            Application.targetFrameRate = _config.TargetFps;
            Debug.unityLogger.logEnabled = _config.IsLogEnabled;
            Input.multiTouchEnabled = false;
            CultureInfo.CurrentCulture = CreateProjectCultureInfo();

            _persistenceStartup.Run();
            await UniTask.Yield(ct);

            await _remoteConfigsService.InitializeAsync(ct);
            _analyticsStartup.Run();

            _pushNotificationsService.Init(ct);

            _levelsLoadService.Init(_loadingScreen);
            _levelsLoadService.Run();
            await _levelsLoadService.LoadLevelAsync();
        }


        private static CultureInfo CreateProjectCultureInfo()
        {
            CultureInfo cultureInfo = (CultureInfo)CultureInfo.InvariantCulture.Clone();
            cultureInfo.NumberFormat.PercentPositivePattern = 1;
            return cultureInfo;
        }
    }
}
