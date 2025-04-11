using System;


namespace Honeylab.Utils.Analytics
{
    public interface IAppStateTracker : IDisposable
    {
        void Run();
    }
}
