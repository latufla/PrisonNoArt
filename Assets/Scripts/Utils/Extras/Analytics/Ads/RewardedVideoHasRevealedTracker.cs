using Honeylab.Utils.Ads;
using static Honeylab.Utils.Analytics.CommonAnalytics.Events;


namespace Honeylab.Utils.Analytics
{
    public class RewardedVideoHasRevealedTracker : IAdsStateTracker
    {
        private readonly IRewardedVideoService _rewardedVideoService;
        private readonly IAnalyticsService _analyticsService;


        public RewardedVideoHasRevealedTracker(IRewardedVideoService rewardedVideoService,
            IAnalyticsService analyticsService)
        {
            _rewardedVideoService = rewardedVideoService;
            _analyticsService = analyticsService;
        }


        public void Run() => _rewardedVideoService.RewardedVideoHasRevealed +=
            RewardedVideoService_OnRewardedVideoHasRevealed;


        public void Dispose() => _rewardedVideoService.RewardedVideoHasRevealed -=
            RewardedVideoService_OnRewardedVideoHasRevealed;


        private void RewardedVideoService_OnRewardedVideoHasRevealed(RewardedVideoHasRevealedEventArgs args)
        {
            var message = AdsMessages.CreateMessage(args);
            _analyticsService.ReportEvent(RewardedVideoHasRevealed, message);
            _analyticsService.SendEventsBuffer();
        }
    }
}
