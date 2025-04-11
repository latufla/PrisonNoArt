using System;


namespace Honeylab.Utils.Ads
{
    public interface IInterstitialService
    {
        event Action<InterstitialStartRequestedEventArgs> InterstitialStartRequested;
        event Action<InterstitialHasRevealedEventArgs> InterstitialHasRevealed;
        event Action<InterstitialRevealFailedEventArgs> InterstitialRevealFailed;
        event Action<InterstitialClickedEventArgs> InterstitialClicked;
        event Action<InterstitialDismissedEventArgs> InterstitialDismissed;
        event Action<InterstitialLoadFailedEventArgs> InterstitialLoadFailed;

        bool IsAvailable { get; }

        InterstitialStartResult StartInterstitial(string placement);
    }
}
