using Honeylab.Utils.Ads;
using static Honeylab.Utils.Analytics.CommonAnalytics.Events;


namespace Honeylab.Utils.Analytics
{
    public class InterstitialLoadFailedTracker : IAdsStateTracker
    {
        private readonly IInterstitialService _interstitialService;
        private readonly IAnalyticsService _analyticsService;


        public InterstitialLoadFailedTracker(IInterstitialService interstitialService,
            IAnalyticsService analyticsService)
        {
            _interstitialService = interstitialService;
            _analyticsService = analyticsService;
        }


        public void Run() => _interstitialService.InterstitialLoadFailed +=
            InterstitialService_OnInterstitialLoadFailed;


        public void Dispose() => _interstitialService.InterstitialLoadFailed -=
            InterstitialService_OnInterstitialLoadFailed;


        private void InterstitialService_OnInterstitialLoadFailed(InterstitialLoadFailedEventArgs args)
        {
            var message = AdsMessages.CreateMessage(args);
            _analyticsService.ReportEvent(InterstitialLoadFailed, message);
            _analyticsService.SendEventsBuffer();
        }
    }
}
