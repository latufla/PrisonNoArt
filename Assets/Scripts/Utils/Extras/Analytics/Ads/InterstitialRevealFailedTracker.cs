using Honeylab.Utils.Ads;
using static Honeylab.Utils.Analytics.CommonAnalytics.Events;


namespace Honeylab.Utils.Analytics
{
    public class InterstitialRevealFailedTracker : IAdsStateTracker
    {
        private readonly IInterstitialService _interstitialService;
        private readonly IAnalyticsService _analyticsService;


        public InterstitialRevealFailedTracker(IInterstitialService interstitialService,
            IAnalyticsService analyticsService)
        {
            _interstitialService = interstitialService;
            _analyticsService = analyticsService;
        }


        public void Run() => _interstitialService.InterstitialRevealFailed +=
            InterstitialService_OnInterstitialRevealFailed;


        public void Dispose() => _interstitialService.InterstitialRevealFailed -=
            InterstitialService_OnInterstitialRevealFailed;


        private void InterstitialService_OnInterstitialRevealFailed(InterstitialRevealFailedEventArgs args)
        {
            var message = AdsMessages.CreateMessage(args);
            _analyticsService.ReportEvent(InterstitialRevealFailed, message);
            _analyticsService.SendEventsBuffer();
        }
    }
}
