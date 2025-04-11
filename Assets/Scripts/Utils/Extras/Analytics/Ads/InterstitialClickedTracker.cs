using Honeylab.Utils.Ads;
using static Honeylab.Utils.Analytics.CommonAnalytics.Events;


namespace Honeylab.Utils.Analytics
{
    public class InterstitialClickedTracker : IAdsStateTracker
    {
        private readonly IInterstitialService _interstitialService;
        private readonly IAnalyticsService _analyticsService;


        public InterstitialClickedTracker(IInterstitialService interstitialService, IAnalyticsService analyticsService)
        {
            _interstitialService = interstitialService;
            _analyticsService = analyticsService;
        }


        public void Run() => _interstitialService.InterstitialClicked += InterstitialService_OnInterstitialClicked;
        public void Dispose() => _interstitialService.InterstitialClicked -= InterstitialService_OnInterstitialClicked;


        private void InterstitialService_OnInterstitialClicked(InterstitialClickedEventArgs args)
        {
            var message = AdsMessages.CreateMessage(args);
            _analyticsService.ReportEvent(InterstitialClicked, message);
            _analyticsService.SendEventsBuffer();
        }
    }
}
