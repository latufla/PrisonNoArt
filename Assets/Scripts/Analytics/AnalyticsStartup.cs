using Honeylab.Utils.Analytics;
using System.Collections.Generic;


namespace Honeylab.Analytics
{
    public class AnalyticsStartup
    {
        private readonly IEnumerable<IAppStateTracker> _appStateTrackers;
        private readonly IEnumerable<IAdsStateTracker> _adsStateTrackers;
        private readonly InterstitialsWatchCountWriteService _interstitialsWatchCountWriteService;
        private readonly RewardedVideosWatchCountService _rewardedVideosWatchCountService;


        public AnalyticsStartup(IEnumerable<IAppStateTracker> appStateTrackers,
            IEnumerable<IAdsStateTracker> adsStateTrackers,
            InterstitialsWatchCountWriteService interstitialsWatchCountWriteService,
            RewardedVideosWatchCountService rewardedVideosWatchCountService)
        {
            _appStateTrackers = appStateTrackers;
            _adsStateTrackers = adsStateTrackers;
            _interstitialsWatchCountWriteService = interstitialsWatchCountWriteService;
            _rewardedVideosWatchCountService = rewardedVideosWatchCountService;
        }


        public void Run()
        {
            _interstitialsWatchCountWriteService.Run();
            _rewardedVideosWatchCountService.Run();

            foreach (IAppStateTracker appStateTracker in _appStateTrackers)
            {
                appStateTracker.Run();
            }

            foreach (IAdsStateTracker adsStateTracker in _adsStateTrackers)
            {
                adsStateTracker.Run();
            }
        }
    }
}
