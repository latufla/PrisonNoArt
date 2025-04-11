using Honeylab.Utils.Ads;
using static Honeylab.Utils.Analytics.CommonAnalytics.Events;


namespace Honeylab.Utils.Analytics
{
    public class BannerClickedTracker : IAdsStateTracker
    {
        private readonly IBannerService _bannerService;
        private readonly IAnalyticsService _analyticsService;


        public BannerClickedTracker(IBannerService bannerService, IAnalyticsService analyticsService)
        {
            _bannerService = bannerService;
            _analyticsService = analyticsService;
        }


        public void Run() => _bannerService.BannerClicked += BannerService_OnBannerClicked;
        public void Dispose() => _bannerService.BannerClicked -= BannerService_OnBannerClicked;


        private void BannerService_OnBannerClicked(BannerClickedEventArgs args)
        {
            var message = AdsMessages.CreateMessage(args);
            _analyticsService.ReportEvent(BannerClicked, message);
        }
    }
}
