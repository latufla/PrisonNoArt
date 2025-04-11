using System;


namespace Honeylab.Utils.App
{
    public interface IAppFocusStateProvider
    {
        event Action<bool> AppFocus;
    }
}
