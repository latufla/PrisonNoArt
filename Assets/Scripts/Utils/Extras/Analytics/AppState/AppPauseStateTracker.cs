using Honeylab.Utils.App;
using System.Collections.Generic;
using static Honeylab.Utils.Analytics.CommonAnalytics.Events;
using static Honeylab.Utils.Analytics.CommonAnalytics.Messages;


namespace Honeylab.Utils.Analytics
{
    public class AppPauseStateTracker : IAppStateTracker
    {
        private readonly IAppPauseStateProvider _appPauseStateProvider;
        private readonly IAnalyticsService _analyticsService;


        public AppPauseStateTracker(IAppPauseStateProvider appPauseStateProvider,
            IAnalyticsService analyticsService)
        {
            _appPauseStateProvider = appPauseStateProvider;
            _analyticsService = analyticsService;
        }


        public void Run() => _appPauseStateProvider.AppPause += AppPauseStateProvider_OnAppPause;
        public void Dispose() => _appPauseStateProvider.AppPause -= AppPauseStateProvider_OnAppPause;


        private void AppPauseStateProvider_OnAppPause(bool isPaused)
        {
            var message = new Dictionary<string, object>
            {
                [IsPaused] = isPaused
            };

            _analyticsService.ReportEvent(AppPause, message);
        }
    }
}
