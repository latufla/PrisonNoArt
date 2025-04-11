using Honeylab.Utils.Prefs;
using System;


namespace Honeylab.Utils.App
{
    public class AppInstallDateTimeService
    {
        private readonly IPrefsService _prefsService;
        private readonly string _appInstallTimestampPrefsKey;
        private DateTimeOffset? _installDateTimeOffsetUtc;


        public AppInstallDateTimeService(IPrefsService prefsService, string appInstallTimestampPrefsKey)
        {
            _prefsService = prefsService;
            _appInstallTimestampPrefsKey = appInstallTimestampPrefsKey;
        }


        public void Run() => _installDateTimeOffsetUtc ??= InitializeInstallDateTimeOffset();


        public DateTime GetAppInstallUtcDateTime() =>
            (_installDateTimeOffsetUtc ??= InitializeInstallDateTimeOffset()).UtcDateTime;


        private DateTimeOffset InitializeInstallDateTimeOffset()
        {
            if (_prefsService.HasKey(_appInstallTimestampPrefsKey))
            {
                long timestampFromPrefs = _prefsService.GetLong(_appInstallTimestampPrefsKey);
                return DateTimeOffset.FromUnixTimeSeconds(timestampFromPrefs);
            }

            DateTimeOffset utcNow = DateTimeOffset.UtcNow;
            long timestampNow = utcNow.ToUnixTimeSeconds();
            _prefsService.SetLong(_appInstallTimestampPrefsKey, timestampNow);
            return utcNow;
        }
    }
}
