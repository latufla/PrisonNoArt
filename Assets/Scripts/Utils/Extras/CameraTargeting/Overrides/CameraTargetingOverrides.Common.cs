using System;
using UnityEngine;


namespace Honeylab.Utils.CameraTargeting
{
    public partial class CameraTargetingOverrides
    {
        private float? _blendDuration;
        private AnimationCurve _easeCurve;
        private Vector3? _followOffset;
        private Vector3? _lookAtOffset;
        private bool? _lookAtEnabled;


        public CameraTargetingOverrides WithBlendDuration(float duration)
        {
            if (duration < 0.0f)
            {
                throw new ArgumentOutOfRangeException(nameof(duration), duration, "Only non-negative values.");
            }

            _blendDuration = duration;
            return this;
        }


        public bool TryGetBlendDuration(out float duration)
        {
            duration = _blendDuration.GetValueOrDefault();
            return _blendDuration.HasValue;
        }


        public CameraTargetingOverrides WithBlendEaseCurve(AnimationCurve curve)
        {
            _easeCurve = curve ?? throw new ArgumentNullException(nameof(curve));
            return this;
        }


        public bool TryGetBlendEaseCurve(out AnimationCurve curve)
        {
            curve = _easeCurve;
            return curve != null;
        }


        public CameraTargetingOverrides WithFollowOffset(Vector3 offset)
        {
            _followOffset = offset;
            return this;
        }


        public bool TryGetFollowOffset(out Vector3 value)
        {
            value = _followOffset.GetValueOrDefault();
            return _followOffset.HasValue;
        }


        public CameraTargetingOverrides WithLookAtEnabled(bool isEnabled)
        {
            _lookAtEnabled = isEnabled;
            return this;
        }


        public bool TryGetLookAtEnabled(out bool isEnabled)
        {
            isEnabled = _lookAtEnabled.GetValueOrDefault();
            return _lookAtEnabled.HasValue;
        }


        public CameraTargetingOverrides WithLookAtOffset(Vector3 offset)
        {
            _lookAtOffset = offset;
            return this;
        }


        public bool TryGetLookAtOffset(out Vector3 value)
        {
            value = _lookAtOffset.GetValueOrDefault();
            return _lookAtOffset.HasValue;
        }
    }
}
