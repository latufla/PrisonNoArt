using Honeylab.Utils.App;


namespace Honeylab.Utils.Analytics
{
    public class HealthCheckTracker : IAppStateTracker
    {
        private const int Interval = 60;
        private readonly AppGlobalTimerService _appGlobalTimerService;
        private readonly IAnalyticsService _analyticsService;
        private readonly IGameLoopUpdateProvider _gameLoopUpdateProvider;
        private int _latestTrackTime;


        public HealthCheckTracker(AppGlobalTimerService appGlobalTimerService,
            IAnalyticsService analyticsService,
            IGameLoopUpdateProvider gameLoopUpdateProvider)
        {
            _appGlobalTimerService = appGlobalTimerService;
            _analyticsService = analyticsService;
            _gameLoopUpdateProvider = gameLoopUpdateProvider;
        }


        public void Run() => _gameLoopUpdateProvider.Updated += GameLoopUpdateProvider_OnUpdated;
        public void Dispose() => _gameLoopUpdateProvider.Updated -= GameLoopUpdateProvider_OnUpdated;


        private void GameLoopUpdateProvider_OnUpdated()
        {
            int now = _appGlobalTimerService.GetSecondsPassedGlobal();
            if (now - _latestTrackTime < Interval)
            {
                return;
            }

            _latestTrackTime = now;
            _analyticsService.ReportEvent(CommonAnalytics.Events.HealthCheck, null);
        }
    }
}
