using MoreMountains.NiceVibrations;
using UnityEngine;


namespace Honeylab.Utils.Haptics
{
    public class NiceVibrationsHapticsService : IHapticsService
    {
        private readonly float _cooldown;
        private float _latestHapticTime;
        private int _latestHapticFrame;


        public NiceVibrationsHapticsService(float cooldown = 0.0f)
        {
            _cooldown = cooldown;
        }


        public void ButtonClick() => PlayHapticIfPossible(HapticTypes.LightImpact);


        private void PlayHapticIfPossible(float intensity, float sharpness)
        {
            if (TryAcquireHapticPermission())
            {
                MMVibrationManager.TransientHaptic(intensity, sharpness);
            }
        }


        private void PlayHapticIfPossible(HapticTypes hapticType)
        {
            if (TryAcquireHapticPermission())
            {
                MMVibrationManager.Haptic(hapticType);
            }
        }


        private bool TryAcquireHapticPermission()
        {
            int frameNow = Time.frameCount;
            if (_latestHapticFrame == frameNow)
            {
                return false;
            }

            float timeNow = Time.realtimeSinceStartup;
            if (timeNow < _latestHapticTime + _cooldown)
            {
                return false;
            }

            _latestHapticTime = timeNow;
            _latestHapticFrame = frameNow;
            return true;
        }
    }
}
