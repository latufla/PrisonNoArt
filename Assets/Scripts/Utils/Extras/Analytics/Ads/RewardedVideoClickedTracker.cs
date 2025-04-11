using Honeylab.Utils.Ads;
using static Honeylab.Utils.Analytics.CommonAnalytics.Events;


namespace Honeylab.Utils.Analytics
{
    public class RewardedVideoClickedTracker : IAdsStateTracker
    {
        private readonly IRewardedVideoService _rewardedVideoService;
        private readonly IAnalyticsService _analyticsService;


        public RewardedVideoClickedTracker(IRewardedVideoService rewardedVideoService,
            IAnalyticsService analyticsService)
        {
            _rewardedVideoService = rewardedVideoService;
            _analyticsService = analyticsService;
        }


        public void Run() => _rewardedVideoService.RewardedVideoClicked += RewardedVideoService_OnRewardedVideoClicked;


        public void Dispose() =>
            _rewardedVideoService.RewardedVideoClicked -= RewardedVideoService_OnRewardedVideoClicked;


        private void RewardedVideoService_OnRewardedVideoClicked(RewardedVideoClickedEventArgs args)
        {
            var message = AdsMessages.CreateMessage(args);
            _analyticsService.ReportEvent(RewardedVideoClicked, message);
            _analyticsService.SendEventsBuffer();
        }
    }
}
