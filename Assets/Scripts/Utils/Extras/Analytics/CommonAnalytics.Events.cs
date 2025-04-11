namespace Honeylab.Utils.Analytics
{
    public static partial class CommonAnalytics
    {
        public static class Events
        {
            public const string InterstitialCheckFailed = "interstitial_check_failed";
            public const string InterstitialStartSucceeded = "interstitial_start_succeeded";
            public const string InterstitialNotReady = "interstitial_not_available";
            public const string InterstitialHasRevealed = "interstitial_start";
            public const string InterstitialRevealFailed = "interstitial_start_failed";
            public const string InterstitialClicked = "interstitial_clicked";
            public const string InterstitialDismissed = "interstitial_shown";
            public const string InterstitialLoadFailed = "interstitial_load_failed";

            public const string RewardedVideoStartSucceeded = "reward_start_succeeded";
            public const string RewardedVideoNotReady = "reward_not_available";
            public const string RewardedVideoHasRevealed = "reward_start";
            public const string RewardedVideoRevealFailed = "reward_start_failed";
            public const string RewardedVideoClicked = "reward_clicked";
            public const string RewardedVideoWatched = "reward_shown";
            public const string RewardedVideoCanceled = "reward_canceled";
            public const string RewardedVideoLoadFailed = "reward_load_failed";

            public const string BannerClicked = "banner_clicked";

            public const string GameStart = "game_start";
            public const string AppStart = "app_start";
            public const string AppPause = "application_pause";
            public const string AppFocus = "application_focus";

            public const string HealthCheck = "healthcheck";
        }
    }
}
