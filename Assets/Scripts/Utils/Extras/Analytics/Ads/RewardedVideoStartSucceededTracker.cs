using Honeylab.Utils.Ads;
using static Honeylab.Utils.Analytics.CommonAnalytics.Events;


namespace Honeylab.Utils.Analytics
{
    public class RewardedVideoStartSucceededTracker : IAdsStateTracker
    {
        private readonly IRewardedVideoService _rewardedVideoService;
        private readonly IAnalyticsService _analyticsService;


        public RewardedVideoStartSucceededTracker(IRewardedVideoService rewardedVideoService,
            IAnalyticsService analyticsService)
        {
            _rewardedVideoService = rewardedVideoService;
            _analyticsService = analyticsService;
        }


        public void Run() => _rewardedVideoService.RewardedVideoStartRequested +=
            RewardedVideoService_OnRewardedVideoStartRequested;


        public void Dispose() => _rewardedVideoService.RewardedVideoStartRequested -=
            RewardedVideoService_OnRewardedVideoStartRequested;


        private void RewardedVideoService_OnRewardedVideoStartRequested(RewardedVideoStartRequestedEventArgs args)
        {
            if (args.Result != RewardedVideoStartResult.Success)
            {
                return;
            }

            var message = AdsMessages.CreateMessage(args);
            _analyticsService.ReportEvent(RewardedVideoStartSucceeded, message);
            _analyticsService.SendEventsBuffer();
        }
    }
}
