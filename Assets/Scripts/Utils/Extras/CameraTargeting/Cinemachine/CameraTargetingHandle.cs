using System;
using UnityEngine;


namespace Honeylab.Utils.CameraTargeting
{
    internal class CameraTargetingHandle : ICameraTargetingHandle
    {
        public Transform Transform { get; }
        public CameraTargetingOverrides Overrides { get; }


        public bool IsFinished { get; private set; }


        public CameraTargetingHandle(Transform transform, CameraTargetingOverrides overrides)
        {
            Transform = transform;
            Overrides = overrides;
        }


        public void StartProcessing()
        {
            if (IsProcessingStarted)
            {
                throw new InvalidOperationException();
            }

            IsProcessingStarted = true;
            ProcessingStarted?.Invoke(this);
        }


        public void MarkFocused()
        {
            if (IsFocused)
            {
                throw new InvalidOperationException();
            }

            IsFocused = true;
            Focused?.Invoke(this);
        }


        public event Action<ICameraTargetingHandle> ProcessingStarted;
        public event Action<ICameraTargetingHandle> Focused;


        public bool IsProcessingStarted { get; private set; }
        public bool IsFocused { get; private set; }


        public void Finish() => IsFinished = true;
    }
}
