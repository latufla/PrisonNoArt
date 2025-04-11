using Cinemachine;
using Cysharp.Threading.Tasks;
using Honeylab.Utils.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;


namespace Honeylab.Utils.CameraTargeting
{
    public class CameraTargetingService : ICameraTargetingService, IDisposable
    {
        private readonly Queue<CameraTargetingHandle> _pendingHandles = new Queue<CameraTargetingHandle>();
        private readonly BlendOverrideStorage _blendOverrideStorage = new BlendOverrideStorage();
        private readonly CinemachineBrain _cinemachineBrain;
        private readonly CinemachineVirtualCamera _primaryCamera;
        private readonly VirtualCamerasPool _virtualCamerasPool;

        private CameraTargetingHandle _runningHandle;


        public CameraTargetingService(CinemachineBrain cinemachineBrain,
            CinemachineVirtualCamera primaryCamera,
            int primaryCameraPriorityOffset = 1)
        {
            _cinemachineBrain = cinemachineBrain;
            _primaryCamera = primaryCamera;
            _virtualCamerasPool = new VirtualCamerasPool(primaryCamera, primaryCameraPriorityOffset);
        }


        public void Run() => _blendOverrideStorage.Run();


        public ICameraTargetingHandle Enqueue(Transform transform) => Enqueue(transform, null);


        public ICameraTargetingHandle Enqueue(Transform transform, CameraTargetingOverrides overrides)
        {
            CameraTargetingHandle handle = new CameraTargetingHandle(transform, overrides);
            if (_runningHandle != null)
            {
                _pendingHandles.Enqueue(handle);
            }
            else
            {
                _runningHandle = handle;
                StartProcessing(handle);
            }

            return handle;
        }


        public bool IsRunningTarget(Transform transform) =>
            _runningHandle != null && _runningHandle.Transform == transform;


        public UniTask WaitForRunningTargetFocusAsync(CancellationToken ct)
        {
            if (_runningHandle == null)
            {
                return UniTask.CompletedTask;
            }

            return _runningHandle.WaitForFocusAsync(ct);
        }


        public void Dispose() => _blendOverrideStorage.Dispose();


        private void StartProcessing(CameraTargetingHandle handle)
        {
            handle.StartProcessing();
            CameraTargetingOverrides overrides = handle.Overrides;
            CinemachineVirtualCamera virtualCamera = _virtualCamerasPool.Pop(handle.Transform, overrides);
            if (overrides != null)
            {
                _blendOverrideStorage.Add(virtualCamera, overrides);
            }

            _cinemachineBrain.ManualUpdate();
            _cinemachineBrain.StartCoroutine(ProcessCoroutine(handle, virtualCamera));
        }


        private IEnumerator ProcessCoroutine(CameraTargetingHandle handle, CinemachineVirtualCamera virtualCamera)
        {
            while (_cinemachineBrain.IsBlending || !_cinemachineBrain.ActiveVirtualCamera.Equals(virtualCamera))
            {
                yield return null;
            }

            handle.MarkFocused();
            while (!handle.IsFinished)
            {
                yield return null;
            }

            virtualCamera.Priority = _primaryCamera.Priority - 1;
            _blendOverrideStorage.Remove(virtualCamera);

            if (_pendingHandles.Count > 0)
            {
                CameraTargetingHandle nextHandle = _pendingHandles.Dequeue();
                _runningHandle = nextHandle;
                StartProcessing(nextHandle);
            }
            else
            {
                _runningHandle = null;
            }

            _cinemachineBrain.StartCoroutine(PushVirtualCameraToPoolCoroutine(virtualCamera));
        }


        private IEnumerator PushVirtualCameraToPoolCoroutine(CinemachineVirtualCamera virtualCamera)
        {
            while (!_cinemachineBrain.IsBlending)
            {
                yield return null;
            }

            while (_cinemachineBrain.ActiveBlend?.Uses(virtualCamera) ?? false)
            {
                yield return null;
            }

            _virtualCamerasPool.Push(virtualCamera);
        }
    }
}
