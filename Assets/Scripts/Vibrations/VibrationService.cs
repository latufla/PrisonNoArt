using Honeylab.Utils.Prefs;
using MoreMountains.NiceVibrations;
using System;
using UniRx;
using UnityEngine;


namespace Honeylab.Utils
{
    [Serializable]
    public class VibrationServiceParams
    {
        public float Cooldown = 0.1f;
    }


    public class VibrationService : IDisposable
    {
        public const string Vibro = nameof(Vibro);
        private readonly ReactiveProperty<bool> _isEnabledProp = new ReactiveProperty<bool>(true);
        private readonly CompositeDisposable _disposable = new CompositeDisposable();

        private readonly VibrationServiceParams _params;
        private readonly IPrefsService _prefsService;
        private float _lastVibrationTime;


        public ReactiveProperty<bool> IsEnabledProp => _isEnabledProp;


        public VibrationService(VibrationServiceParams p, IPrefsService prefsService)
        {
            _params = p;
            _prefsService = prefsService;
        }


        public void Init()
        {
            if (_prefsService.HasKey(Vibro))
            {
                _isEnabledProp.Value = _prefsService.GetBool(Vibro);
            }
        }


        public void Run()
        {
            IDisposable isEnabledDisposable = _isEnabledProp.Subscribe(isEnabled =>
            {
                _prefsService.SetBool(Vibro, isEnabled);
            });
            _disposable.Add(isEnabledDisposable);
        }


        public void Dispose()
        {
            _disposable?.Dispose();
        }


        public void Vibrate()
        {
            VibrateLightImpact();
        }


        public bool IsVibrateReady() => Time.realtimeSinceStartup - _lastVibrationTime >= _params.Cooldown;


        private bool VibrateLightImpact()
        {
            if (!_isEnabledProp.Value)
            {
                return false;
            }

            if (!IsVibrateReady())
            {
                return false;
            }

            _lastVibrationTime = Time.realtimeSinceStartup;

            MMVibrationManager.Haptic(HapticTypes.LightImpact);
            return true;
        }
    }
}
