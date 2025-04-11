using Honeylab.Utils.Ads;
using Honeylab.Utils.Prefs;
using System;


namespace Honeylab.Utils.Analytics
{
    public class RewardedVideosWatchCountService : IDisposable
    {
        private readonly IPrefsService _prefsService;
        private readonly string _prefsKey;
        private readonly IRewardedVideoService _rewardedVideoService;


        public RewardedVideosWatchCountService(IPrefsService prefsService,
            string prefsKey,
            IRewardedVideoService rewardedVideoService)
        {
            _prefsService = prefsService;
            _prefsKey = prefsKey;
            _rewardedVideoService = rewardedVideoService;
        }


        public void Run() =>
            _rewardedVideoService.RewardedVideoDismissed += RewardedVideoService_OnRewardedVideoDismissed;


        public int GetWatchCount() => _prefsService.GetInt(_prefsKey);


        public void Dispose() =>
            _rewardedVideoService.RewardedVideoDismissed -= RewardedVideoService_OnRewardedVideoDismissed;


        private void RewardedVideoService_OnRewardedVideoDismissed(RewardedVideoDismissedEventArgs args)
        {
            if (args.ShowResult != RewardedVideoShowResult.Watched)
            {
                return;
            }

            int oldCount = _prefsService.GetInt(_prefsKey, 0);
            _prefsService.SetInt(_prefsKey, oldCount + 1);
        }
    }
}
