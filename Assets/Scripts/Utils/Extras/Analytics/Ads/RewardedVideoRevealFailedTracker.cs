using Honeylab.Utils.Ads;
using static Honeylab.Utils.Analytics.CommonAnalytics.Events;


namespace Honeylab.Utils.Analytics
{
    public class RewardedVideoRevealFailedTracker : IAdsStateTracker
    {
        private readonly IRewardedVideoService _rewardedVideoService;
        private readonly IAnalyticsService _analyticsService;


        public RewardedVideoRevealFailedTracker(IRewardedVideoService rewardedVideoService,
            IAnalyticsService analyticsService)
        {
            _rewardedVideoService = rewardedVideoService;
            _analyticsService = analyticsService;
        }


        public void Run() => _rewardedVideoService.RewardedVideoRevealFailed +=
            RewardedVideoService_OnRewardedVideoRevealFailed;


        public void Dispose() => _rewardedVideoService.RewardedVideoRevealFailed -=
            RewardedVideoService_OnRewardedVideoRevealFailed;


        private void RewardedVideoService_OnRewardedVideoRevealFailed(RewardedVideoRevealFailedEventArgs args)
        {
            var message = AdsMessages.CreateMessage(args);
            _analyticsService.ReportEvent(RewardedVideoRevealFailed, message);
            _analyticsService.SendEventsBuffer();
        }
    }
}
