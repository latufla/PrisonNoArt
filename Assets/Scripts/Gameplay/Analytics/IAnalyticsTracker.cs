using System;


namespace Honeylab.Gameplay.Analytics
{
    public interface IAnalyticsTracker : IDisposable
    {
        void Run();
    }
}
