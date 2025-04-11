namespace Honeylab.Utils.Ads
{
    public enum InterstitialStartResult
    {
        None = 0,
        Success = 1,
        NotReady = 2,
        TimeoutNotPassed = 3,
        InterstitialsDisabled = 4,
        FullScreenAdsIsAlreadyShowing = 5
    }
}
