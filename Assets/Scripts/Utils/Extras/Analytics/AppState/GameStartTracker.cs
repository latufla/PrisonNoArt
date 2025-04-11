using static Honeylab.Utils.Analytics.CommonAnalytics.Events;


namespace Honeylab.Utils.Analytics
{
    public class GameStartTracker : IAppStateTracker
    {
        private readonly IAnalyticsService _analyticsService;


        public GameStartTracker(IAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
        }


        public void Run() => _analyticsService.ReportEvent(GameStart, null);
        public void Dispose() { }
    }
}
