using System;


namespace Honeylab.Utils.Ads
{
    public class StubRewardedVideoService : IRewardedVideoService
    {
        private readonly RewardedVideoStartResult _startResult;
        private bool _isStartInProgress;


        public StubRewardedVideoService(RewardedVideoStartResult startResult)
        {
            _startResult = startResult;
        }


        public event Action<RewardedVideoStartRequestedEventArgs> RewardedVideoStartRequested;
        public event Action<RewardedVideoHasRevealedEventArgs> RewardedVideoHasRevealed;
        public event Action<RewardedVideoRevealFailedEventArgs> RewardedVideoRevealFailed;
        public event Action<RewardedVideoClickedEventArgs> RewardedVideoClicked;
        public event Action<RewardedVideoDismissedEventArgs> RewardedVideoDismissed;
        public event Action<RewardedVideoLoadFailedEventArgs> RewardedVideoLoadFailed;


        public bool IsAvailable => !_isStartInProgress && _startResult == RewardedVideoStartResult.Success;


        public RewardedVideoStartResult StartRewardedVideo(string placement)
        {
            _isStartInProgress = true;

            RewardedVideoStartRequested?.Invoke(
                new RewardedVideoStartRequestedEventArgs(this, placement, _startResult));
            if (_startResult != RewardedVideoStartResult.Success)
            {
                _isStartInProgress = false;
                return _startResult;
            }

            RewardedVideoHasRevealed?.Invoke(new RewardedVideoHasRevealedEventArgs(this, placement));
            RewardedVideoDismissed?.Invoke(
                new RewardedVideoDismissedEventArgs(this, placement, RewardedVideoShowResult.Watched));

            _isStartInProgress = false;
            return _startResult;
        }
    }
}
