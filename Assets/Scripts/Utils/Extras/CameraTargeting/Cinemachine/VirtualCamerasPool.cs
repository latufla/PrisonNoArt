using Cinemachine;
using System.Collections.Generic;
using UnityEngine;


namespace Honeylab.Utils.CameraTargeting
{
    internal class VirtualCamerasPool
    {
        private readonly Stack<CinemachineVirtualCamera> _pooledVirtualCameras = new Stack<CinemachineVirtualCamera>();
        private readonly VirtualCameraComponentsCache _componentsCache = new VirtualCameraComponentsCache();
        private readonly CinemachineVirtualCamera _prototype;
        private readonly int _prototypeCameraPriorityOffset;
        private int _instantiatedCameraCount;


        public VirtualCamerasPool(CinemachineVirtualCamera prototype, int prototypeCameraPriorityOffset)
        {
            _prototype = prototype;
            _prototypeCameraPriorityOffset = prototypeCameraPriorityOffset;
        }


        public CinemachineVirtualCamera Pop(Transform transform, CameraTargetingOverrides overrides)
        {
            CinemachineVirtualCamera virtualCamera =
                _pooledVirtualCameras.Count > 0 ? _pooledVirtualCameras.Pop() : InstantiateNewCamera();
            virtualCamera.Follow = transform;
            virtualCamera.LookAt = ResolveLookAt(transform, overrides);
            SetupVirtualCamera(virtualCamera, overrides);
            virtualCamera.gameObject.SetActive(true);
            return virtualCamera;
        }


        public void Push(CinemachineVirtualCamera virtualCamera)
        {
            virtualCamera.gameObject.SetActive(false);
            virtualCamera.Follow = null;
            virtualCamera.LookAt = null;
            _pooledVirtualCameras.Push(virtualCamera);
        }


        private CinemachineVirtualCamera InstantiateNewCamera()
        {
            CinemachineVirtualCamera virtualCamera =
                Object.Instantiate(_prototype, _prototype.transform.parent);
            virtualCamera.gameObject.name = $"{nameof(VirtualCamerasPool)}_vcam_{_instantiatedCameraCount++:D2}";
            return virtualCamera;
        }


        private Transform ResolveLookAt(Transform transform, CameraTargetingOverrides overrides)
        {
            if (overrides == null || !overrides.TryGetLookAtEnabled(out bool isLookAtEnabled))
            {
                return _prototype.LookAt != null ? transform : null;
            }

            return isLookAtEnabled ? transform : null;
        }


        private void SetupVirtualCamera(CinemachineVirtualCamera virtualCamera, CameraTargetingOverrides overrides)
        {
            if (_componentsCache.TryGetComponent(virtualCamera, out CinemachineTransposer transposer) &&
                _componentsCache.TryGetComponent(_prototype, out CinemachineTransposer prototypeTransposer))
            {
                SetupTransposer(transposer, prototypeTransposer, overrides);
            }

            if (_componentsCache.TryGetComponent(virtualCamera, out CinemachineFramingTransposer framingTransposer) &&
                _componentsCache.TryGetComponent(_prototype,
                    out CinemachineFramingTransposer prototypeFramingTransposer))
            {
                SetupFramingTransposer(framingTransposer, prototypeFramingTransposer, overrides);
            }

            if (_componentsCache.TryGetComponent(virtualCamera, out CinemachineComposer composer) &&
                _componentsCache.TryGetComponent(_prototype, out CinemachineComposer prototypeComposer))
            {
                SetupComposer(composer, prototypeComposer, overrides);
            }

            virtualCamera.m_Lens = CreateLensSettings(_prototype.m_Lens, overrides);
            virtualCamera.Priority = _prototype.Priority + _prototypeCameraPriorityOffset;
        }


        private static void SetupTransposer(CinemachineTransposer transposer,
            CinemachineTransposer prototypeTransposer,
            CameraTargetingOverrides overrides)
        {
            transposer.m_FollowOffset = overrides != null && overrides.TryGetFollowOffset(out Vector3 overridenOffset) ?
                overridenOffset :
                prototypeTransposer.m_FollowOffset;
        }


        private static void SetupFramingTransposer(CinemachineFramingTransposer framingTransposer,
            CinemachineFramingTransposer prototypeFramingTransposer,
            CameraTargetingOverrides overrides)
        {
            framingTransposer.m_TrackedObjectOffset =
                overrides != null && overrides.TryGetFollowOffset(out Vector3 overridenOffset) ?
                    overridenOffset :
                    prototypeFramingTransposer.m_TrackedObjectOffset;

            framingTransposer.m_CameraDistance =
                overrides != null && overrides.TryGetCameraDistance(out float overridenDistance) ?
                    overridenDistance :
                    prototypeFramingTransposer.m_CameraDistance;

            Transform virtualCameraTransform = framingTransposer.VirtualCamera.transform;
            Transform prototypeTransform = prototypeFramingTransposer.VirtualCamera.transform;
            SetupRotation(virtualCameraTransform, prototypeTransform, overrides);


            static void SetupRotation(Transform virtualCameraTransform,
                Transform prototypeTransform,
                CameraTargetingOverrides overrides)
            {
                if (overrides != null)
                {
                    if (overrides.TryGetRotation(out Quaternion rotation))
                    {
                        virtualCameraTransform.rotation = rotation;
                        return;
                    }

                    if (overrides.TryGetLocalRotation(out Quaternion localRotation))
                    {
                        virtualCameraTransform.localRotation = localRotation;
                        return;
                    }
                }

                virtualCameraTransform.rotation = prototypeTransform.rotation;
            }
        }


        private static void SetupComposer(CinemachineComposer composer,
            CinemachineComposer prototypeComposer,
            CameraTargetingOverrides overrides)
        {
            composer.m_TrackedObjectOffset =
                overrides != null && overrides.TryGetLookAtOffset(out Vector3 lookAtOffset) ?
                    lookAtOffset :
                    prototypeComposer.m_TrackedObjectOffset;
        }


        private static LensSettings CreateLensSettings(LensSettings prototypeSettings,
            CameraTargetingOverrides overrides)
        {
            if (overrides == null)
            {
                return prototypeSettings;
            }

            LensSettings result = prototypeSettings;
            if (overrides.TryGetOrthoSize(out float orthoSize))
            {
                result.OrthographicSize = orthoSize;
            }

            if (overrides.TryGetFieldOfView(out float fieldOfView))
            {
                result.FieldOfView = fieldOfView;
            }

            return result;
        }
    }
}
