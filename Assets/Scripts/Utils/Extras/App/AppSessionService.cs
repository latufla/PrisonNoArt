using Honeylab.Utils.Prefs;
using System;
using UnityEngine;


namespace Honeylab.Utils.App
{
    public class AppSessionService : IDisposable
    {
        private readonly IPrefsService _prefsService;
        private readonly string _sessionCountPrefsKey;
        private readonly IAppPauseStateProvider _appPauseStateProvider;
        private readonly double _sessionBumpAppPauseMinutes;

        private AppSessionDef? _activeSessionDef;
        private AppSessionPauseContext? _activeSessionPauseContext;


        public AppSessionService(IPrefsService prefsService,
            string sessionCountPrefsKey,
            IAppPauseStateProvider appPauseStateProvider,
            double sessionBumpAppPauseMinutes)
        {
            _prefsService = prefsService;
            _sessionCountPrefsKey = sessionCountPrefsKey;
            _appPauseStateProvider = appPauseStateProvider;
            _sessionBumpAppPauseMinutes = sessionBumpAppPauseMinutes;
        }


        public void Run()
        {
            _activeSessionDef ??= StartNewSession(Time.time);
            _appPauseStateProvider.AppPause += AppPauseStateProvider_OnAppPause;
        }


        public int GetSessionNumber() => (_activeSessionDef ??= StartNewSession(Time.time)).Number;


        public int GetSecondsPassedSession()
        {
            float gameTimeNow = Time.time;
            _activeSessionDef ??= StartNewSession(gameTimeNow);
            return Mathf.FloorToInt(gameTimeNow - _activeSessionDef.Value.StartGameTime);
        }


        public void Dispose() => _appPauseStateProvider.AppPause -= AppPauseStateProvider_OnAppPause;


        private AppSessionDef StartNewSession(float startGameTime)
        {
            int previousSessionCount = _prefsService.GetInt(_sessionCountPrefsKey, 0);
            int newSessionsCount = previousSessionCount + 1;
            _prefsService.SetInt(_sessionCountPrefsKey, newSessionsCount);
            return new AppSessionDef(newSessionsCount, startGameTime);
        }


        private void AppPauseStateProvider_OnAppPause(bool isPaused)
        {
            if (isPaused)
            {
                _activeSessionPauseContext ??= new AppSessionPauseContext(Time.time, DateTimeOffset.UtcNow);
            }
            else if (_activeSessionPauseContext.HasValue)
            {
                AppSessionPauseContext activeSessionPauseContext = _activeSessionPauseContext.Value;
                DateTimeOffset unpauseDateTimeOffsetUtc = DateTimeOffset.UtcNow;

                if ((unpauseDateTimeOffsetUtc - activeSessionPauseContext.DateTimeOffsetUtc).TotalMinutes >=
                    _sessionBumpAppPauseMinutes)
                {
                    _activeSessionDef = StartNewSession(activeSessionPauseContext.GameTime);
                }

                _activeSessionPauseContext = null;
            }
        }
    }
}
