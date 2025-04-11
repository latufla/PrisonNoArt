using Honeylab.Utils.Ads;
using static Honeylab.Utils.Analytics.CommonAnalytics.Events;


namespace Honeylab.Utils.Analytics
{
    public class InterstitialHasRevealedTracker : IAdsStateTracker
    {
        private readonly IInterstitialService _interstitialService;
        private readonly IAnalyticsService _analyticsService;


        public InterstitialHasRevealedTracker(IInterstitialService interstitialService,
            IAnalyticsService analyticsService)
        {
            _interstitialService = interstitialService;
            _analyticsService = analyticsService;
        }


        public void Run() => _interstitialService.InterstitialHasRevealed +=
            InterstitialService_OnInterstitialHasRevealed;


        public void Dispose() => _interstitialService.InterstitialHasRevealed -=
            InterstitialService_OnInterstitialHasRevealed;


        private void InterstitialService_OnInterstitialHasRevealed(InterstitialHasRevealedEventArgs args)
        {
            var message = AdsMessages.CreateMessage(args);
            _analyticsService.ReportEvent(InterstitialHasRevealed, message);
            _analyticsService.SendEventsBuffer();
        }
    }
}
