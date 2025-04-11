using System;


namespace Honeylab.Utils.Persistence
{
    public interface IAutoSaveService : IDisposable
    {
        void Run();
    }
}
