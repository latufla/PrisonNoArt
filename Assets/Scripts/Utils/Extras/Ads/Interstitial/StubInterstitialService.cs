using System;


namespace Honeylab.Utils.Ads
{
    public class StubInterstitialService : IInterstitialService
    {
        private readonly InterstitialStartResult _startResult;
        private bool _isStartInProgress;


        public StubInterstitialService(InterstitialStartResult startResult)
        {
            _startResult = startResult;
        }


        public event Action<InterstitialStartRequestedEventArgs> InterstitialStartRequested;
        public event Action<InterstitialHasRevealedEventArgs> InterstitialHasRevealed;
        public event Action<InterstitialRevealFailedEventArgs> InterstitialRevealFailed;
        public event Action<InterstitialClickedEventArgs> InterstitialClicked;
        public event Action<InterstitialDismissedEventArgs> InterstitialDismissed;
        public event Action<InterstitialLoadFailedEventArgs> InterstitialLoadFailed;


        public bool IsAvailable => !_isStartInProgress && _startResult == InterstitialStartResult.Success;


        public InterstitialStartResult StartInterstitial(string placement)
        {
            _isStartInProgress = true;

            InterstitialStartRequested?.Invoke(new InterstitialStartRequestedEventArgs(this, placement, _startResult));
            if (_startResult != InterstitialStartResult.Success)
            {
                _isStartInProgress = false;
                return _startResult;
            }

            InterstitialHasRevealed?.Invoke(new InterstitialHasRevealedEventArgs(this, placement));
            InterstitialDismissed?.Invoke(new InterstitialDismissedEventArgs(this, placement));

            _isStartInProgress = false;
            return _startResult;
        }
    }
}
