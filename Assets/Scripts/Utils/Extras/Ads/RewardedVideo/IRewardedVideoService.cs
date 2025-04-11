using System;


namespace Honeylab.Utils.Ads
{
    public interface IRewardedVideoService
    {
        event Action<RewardedVideoStartRequestedEventArgs> RewardedVideoStartRequested;
        event Action<RewardedVideoHasRevealedEventArgs> RewardedVideoHasRevealed;
        event Action<RewardedVideoRevealFailedEventArgs> RewardedVideoRevealFailed;
        event Action<RewardedVideoClickedEventArgs> RewardedVideoClicked;
        event Action<RewardedVideoDismissedEventArgs> RewardedVideoDismissed;
        event Action<RewardedVideoLoadFailedEventArgs> RewardedVideoLoadFailed;

        bool IsAvailable { get; }

        RewardedVideoStartResult StartRewardedVideo(string placement);
    }
}
