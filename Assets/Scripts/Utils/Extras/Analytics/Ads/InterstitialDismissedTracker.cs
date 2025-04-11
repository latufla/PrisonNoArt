using Honeylab.Utils.Ads;
using static Honeylab.Utils.Analytics.CommonAnalytics.Events;


namespace Honeylab.Utils.Analytics
{
    public class InterstitialDismissedTracker : IAdsStateTracker
    {
        private readonly IInterstitialService _interstitialService;
        private readonly IAnalyticsService _analyticsService;


        public InterstitialDismissedTracker(IInterstitialService interstitialService,
            IAnalyticsService analyticsService)
        {
            _interstitialService = interstitialService;
            _analyticsService = analyticsService;
        }


        public void Run() => _interstitialService.InterstitialDismissed +=
            InterstitialService_OnInterstitialDismissed;


        public void Dispose() => _interstitialService.InterstitialDismissed -=
            InterstitialService_OnInterstitialDismissed;


        private void InterstitialService_OnInterstitialDismissed(InterstitialDismissedEventArgs args)
        {
            var message = AdsMessages.CreateMessage(args);
            _analyticsService.ReportEvent(InterstitialDismissed, message);
            _analyticsService.SendEventsBuffer();
        }
    }
}
