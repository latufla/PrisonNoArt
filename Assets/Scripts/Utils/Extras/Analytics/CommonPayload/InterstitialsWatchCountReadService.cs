using Honeylab.Utils.Prefs;


namespace Honeylab.Utils.Analytics
{
    public class InterstitialsWatchCountReadService
    {
        private readonly IPrefsService _prefsService;
        private readonly string _prefsKey;


        public InterstitialsWatchCountReadService(IPrefsService prefsService, string prefsKey)
        {
            _prefsService = prefsService;
            _prefsKey = prefsKey;
        }


        public int GetWatchCount() => _prefsService.GetInt(_prefsKey);
    }
}
