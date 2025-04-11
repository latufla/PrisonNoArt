using Honeylab.Utils.Ads;
using Honeylab.Utils.Prefs;
using System;


namespace Honeylab.Utils.Analytics
{
    public class InterstitialsWatchCountWriteService : IDisposable
    {
        private readonly IPrefsService _prefsService;
        private readonly string _prefsKey;
        private readonly IInterstitialService _interstitialService;


        public InterstitialsWatchCountWriteService(IPrefsService prefsService,
            string prefsKey,
            IInterstitialService interstitialService)
        {
            _prefsService = prefsService;
            _prefsKey = prefsKey;
            _interstitialService = interstitialService;
        }


        public void Run() => _interstitialService.InterstitialDismissed += InterstitialService_OnInterstitialDismissed;


        public void Dispose() =>
            _interstitialService.InterstitialDismissed -= InterstitialService_OnInterstitialDismissed;


        private void InterstitialService_OnInterstitialDismissed(InterstitialDismissedEventArgs obj)
        {
            int oldCount = _prefsService.GetInt(_prefsKey, 0);
            _prefsService.SetInt(_prefsKey, oldCount + 1);
        }
    }
}
