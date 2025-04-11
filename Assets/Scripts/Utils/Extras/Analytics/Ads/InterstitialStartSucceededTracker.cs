using Honeylab.Utils.Ads;
using static Honeylab.Utils.Analytics.CommonAnalytics.Events;


namespace Honeylab.Utils.Analytics
{
    public class InterstitialStartSucceededTracker : IAdsStateTracker
    {
        private readonly IInterstitialService _interstitialService;
        private readonly IAnalyticsService _analyticsService;


        public InterstitialStartSucceededTracker(IInterstitialService interstitialService,
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
            if (args.Result != InterstitialStartResult.Success)
            {
                return;
            }

            var message = AdsMessages.CreateMessage(args);
            _analyticsService.ReportEvent(InterstitialStartSucceeded, message);
            _analyticsService.SendEventsBuffer();
        }
    }
}
