using System;


namespace Honeylab.Utils.App
{
    public interface IAppPauseStateProvider
    {
        event Action<bool> AppPause;
    }
}
