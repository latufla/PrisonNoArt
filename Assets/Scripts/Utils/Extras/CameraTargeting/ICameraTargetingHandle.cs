using System;


namespace Honeylab.Utils.CameraTargeting
{
    public interface ICameraTargetingHandle
    {
        event Action<ICameraTargetingHandle> ProcessingStarted;
        event Action<ICameraTargetingHandle> Focused;

        bool IsProcessingStarted { get; }
        bool IsFocused { get; }

        void Finish();
    }
}
