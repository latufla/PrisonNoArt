using Honeylab.Utils.Ads;
using static Honeylab.Utils.Analytics.CommonAnalytics.Events;


namespace Honeylab.Utils.Analytics
{
    public class InterstitialNotReadyTracker : IAdsStateTracker
    {
        private readonly IInterstitialService _interstitialService;
        private readonly IAnalyticsService _analyticsService;


        public InterstitialNotReadyTracker(IInterstitialService interstitialService,
            IAnalyticsService analyticsService)
        {
            _interstitialService = interstitialService;
            _analyticsService = analyticsService;
        }


        public void Run() => _interstitialService.InterstitialStartRequested +=
            InterstitialService_OnInterstitialStartRequested;


        public void Dispose() => _interstitialService.InterstitialStartRequested -=
            InterstitialService_OnInterstitialStartRequested;


        private void InterstitialService_OnInterstitialStartRequested(InterstitialStartRequestedEventArgs args)
        {
            if (args.Result != InterstitialStartResult.NotReady)
            {
                return;
            }

            var message = AdsMessages.CreateMessage(args);
            _analyticsService.ReportEvent(InterstitialNotReady, message);
            _analyticsService.SendEventsBuffer();
        }
    }
}
