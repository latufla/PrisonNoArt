using static Honeylab.Utils.Analytics.CommonAnalytics.Events;


namespace Honeylab.Utils.Analytics
{
    public class AppStartTracker : IAppStateTracker
    {
        private readonly IAnalyticsService _analyticsService;


        public AppStartTracker(IAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
        }


        public void Run() => _analyticsService.ReportEvent(AppStart, null);
        public void Dispose() { }
    }
}
