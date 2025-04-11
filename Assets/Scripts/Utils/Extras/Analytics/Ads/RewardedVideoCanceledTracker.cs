using Honeylab.Utils.Ads;
using static Honeylab.Utils.Analytics.CommonAnalytics.Events;


namespace Honeylab.Utils.Analytics
{
    public class RewardedVideoCanceledTracker : IAdsStateTracker
    {
        private readonly IRewardedVideoService _rewardedVideoService;
        private readonly IAnalyticsService _analyticsService;


        public RewardedVideoCanceledTracker(IRewardedVideoService rewardedVideoService,
            IAnalyticsService analyticsService)
        {
            _rewardedVideoService = rewardedVideoService;
            _analyticsService = analyticsService;
        }


        public void Run() => _rewardedVideoService.RewardedVideoDismissed +=
            RewardedVideoService_OnRewardedVideoDismissed;


        public void Dispose() => _rewardedVideoService.RewardedVideoDismissed -=
            RewardedVideoService_OnRewardedVideoDismissed;


        private void RewardedVideoService_OnRewardedVideoDismissed(RewardedVideoDismissedEventArgs args)
        {
            if (args.ShowResult != RewardedVideoShowResult.Canceled)
            {
                return;
            }

            var message = AdsMessages.CreateMessage(args);
            _analyticsService.ReportEvent(RewardedVideoCanceled, message);
            _analyticsService.SendEventsBuffer();
        }
    }
}
