using Honeylab.Utils.App;
using System.Collections.Generic;
using static Honeylab.Utils.Analytics.CommonAnalytics.Events;
using static Honeylab.Utils.Analytics.CommonAnalytics.Messages;


namespace Honeylab.Utils.Analytics
{
    public class AppFocusStateTracker : IAppStateTracker
    {
        private readonly IAppFocusStateProvider _appFocusStateProvider;
        private readonly IAnalyticsService _analyticsService;


        public AppFocusStateTracker(IAppFocusStateProvider appFocusStateProvider,
            IAnalyticsService analyticsService)
        {
            _appFocusStateProvider = appFocusStateProvider;
            _analyticsService = analyticsService;
        }


        public void Run() => _appFocusStateProvider.AppFocus += AppFocusStateProvider_OnAppFocus;
        public void Dispose() => _appFocusStateProvider.AppFocus -= AppFocusStateProvider_OnAppFocus;


        private void AppFocusStateProvider_OnAppFocus(bool isFocused)
        {
            var message = new Dictionary<string, object>
            {
                [IsFocused] = isFocused
            };

            _analyticsService.ReportEvent(AppFocus, message);
        }
    }
}
