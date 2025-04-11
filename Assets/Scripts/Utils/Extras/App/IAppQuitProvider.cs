using System;


namespace Honeylab.Utils.App
{
    public interface IAppQuitProvider
    {
        event Action AppQuit;
    }
}
