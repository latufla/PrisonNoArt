using System;


namespace Honeylab.Utils.App
{
    public interface IGameLoopUpdateProvider
    {
        event Action Updated;
    }
}
