using System;
using UnityEngine;


namespace Honeylab.Utils.App
{
    public class AppStateProviderBehaviour : MonoBehaviour,
        IAppPauseStateProvider,
        IAppFocusStateProvider,
        IAppQuitProvider,
        IGameLoopUpdateProvider
    {
        public event Action<bool> AppPause;
        public event Action<bool> AppFocus;
        public event Action AppQuit;
        public event Action Updated;


        private void OnApplicationPause(bool pauseStatus) => AppPause?.Invoke(pauseStatus);
        private void OnApplicationFocus(bool hasFocus) => AppFocus?.Invoke(hasFocus);
        private void OnApplicationQuit() => AppQuit?.Invoke();
        private void Update() => Updated?.Invoke();
    }
}
