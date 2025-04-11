using System;
using UnityEngine;


namespace Honeylab.Utils.CameraTargeting
{
    public partial class CameraTargetingOverrides
    {
        private float? _cameraDistance;
        private Quaternion? _rotation;
        private Quaternion? _localRotation;


        public CameraTargetingOverrides WithCameraDistance(float distance)
        {
            _cameraDistance = distance;
            return this;
        }


        public bool TryGetCameraDistance(out float distance)
        {
            distance = _cameraDistance.GetValueOrDefault();
            return _cameraDistance.HasValue;
        }


        public CameraTargetingOverrides WithRotation(Quaternion rotation)
        {
            if (_localRotation.HasValue)
            {
                throw new InvalidOperationException($"{nameof(_localRotation)} is already set.");
            }

            _rotation = rotation;
            return this;
        }


        public bool TryGetRotation(out Quaternion rotation)
        {
            rotation = _rotation.GetValueOrDefault();
            return _rotation.HasValue;
        }


        public CameraTargetingOverrides WithLocalRotation(Quaternion localRotation)
        {
            if (_rotation.HasValue)
            {
                throw new InvalidOperationException($"{nameof(_rotation)} is already set.");
            }

            _localRotation = localRotation;
            return this;
        }


        public bool TryGetLocalRotation(out Quaternion localRotation)
        {
            localRotation = _localRotation.GetValueOrDefault();
            return _localRotation.HasValue;
        }
    }
}
