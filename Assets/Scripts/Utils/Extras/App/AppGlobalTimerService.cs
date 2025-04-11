using Honeylab.Utils.Prefs;
using System;
using UnityEngine;


namespace Honeylab.Utils.App
{
    public class AppGlobalTimerService : IDisposable
    {
        private readonly IPrefsService _prefsService;
        private readonly string _flushedSecondsPassedGlobalPrefsKey;
        private readonly IAppPauseStateProvider _appPauseStateProvider;
        private readonly IAppFocusStateProvider _appFocusStateProvider;
        private float _flushGameTime;


        public AppGlobalTimerService(IPrefsService prefsService,
            string flushedSecondsPassedGlobalPrefsKey,
            IAppPauseStateProvider appPauseStateProvider,
            IAppFocusStateProvider appFocusStateProvider)
        {
            _prefsService = prefsService;
            _flushedSecondsPassedGlobalPrefsKey = flushedSecondsPassedGlobalPrefsKey;
            _appPauseStateProvider = appPauseStateProvider;
            _appFocusStateProvider = appFocusStateProvider;
            _flushGameTime = Time.time;
        }


        public void Run()
        {
            _appPauseStateProvider.AppPause += AppPauseStateProvider_OnAppPause;
            _appFocusStateProvider.AppFocus += AppFocusStateProvider_OnAppFocus;
        }


        public int GetSecondsPassedGlobal() => Mathf.FloorToInt(Time.time - _flushGameTime) +
            _prefsService.GetInt(_flushedSecondsPassedGlobalPrefsKey);


        public void Dispose()
        {
            _appPauseStateProvider.AppPause -= AppPauseStateProvider_OnAppPause;
            _appFocusStateProvider.AppFocus -= AppFocusStateProvider_OnAppFocus;
        }


        private void Flush(float gameTimeNow)
        {
            int secondsPassedAfterFlush = Mathf.FloorToInt(gameTimeNow - _flushGameTime);

            int oldFlushedSecondsPassedGlobal = _prefsService.GetInt(_flushedSecondsPassedGlobalPrefsKey);
            int newFlushedSecondsPassedGlobal = oldFlushedSecondsPassedGlobal + secondsPassedAfterFlush;
            if (oldFlushedSecondsPassedGlobal < newFlushedSecondsPassedGlobal)
            {
                _prefsService.SetInt(_flushedSecondsPassedGlobalPrefsKey, newFlushedSecondsPassedGlobal);
                _flushGameTime = gameTimeNow;
            }
        }


        private void AppPauseStateProvider_OnAppPause(bool isPaused)
        {
            if (isPaused)
            {
                Flush(Time.time);
            }
        }


        private void AppFocusStateProvider_OnAppFocus(bool isFocused)
        {
            if (!isFocused)
            {
                Flush(Time.time);
            }
        }
    }
}
