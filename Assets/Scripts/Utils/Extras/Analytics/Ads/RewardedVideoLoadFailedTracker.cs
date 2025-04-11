using Honeylab.Utils.Ads;
using static Honeylab.Utils.Analytics.CommonAnalytics.Events;


namespace Honeylab.Utils.Analytics
{
    public class RewardedVideoLoadFailedTracker : IAdsStateTracker
    {
        private readonly IRewardedVideoService _rewardedVideoService;
        private readonly IAnalyticsService _analyticsService;


        public RewardedVideoLoadFailedTracker(IRewardedVideoService rewardedVideoService,
            IAnalyticsService analyticsService)
        {
            _rewardedVideoService = rewardedVideoService;
            _analyticsService = analyticsService;
        }


        public void Run() => _rewardedVideoService.RewardedVideoLoadFailed +=
            RewardedVideoService_OnRewardedVideoLoadFailed;


        public void Dispose() => _rewardedVideoService.RewardedVideoLoadFailed -=
            RewardedVideoService_OnRewardedVideoLoadFailed;


        private void RewardedVideoService_OnRewardedVideoLoadFailed(RewardedVideoLoadFailedEventArgs args)
        {
            var message = AdsMessages.CreateMessage(args);
            _analyticsService.ReportEvent(RewardedVideoLoadFailed, message);
            _analyticsService.SendEventsBuffer();
        }
    }
}
