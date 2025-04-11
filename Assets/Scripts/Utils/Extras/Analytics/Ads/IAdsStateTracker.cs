using System;


namespace Honeylab.Utils.Analytics
{
    public interface IAdsStateTracker : IDisposable
    {
        void Run();
    }
}
