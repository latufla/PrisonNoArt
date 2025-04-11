using System;
using UnityEngine;


namespace Honeylab.FastFury.Services.GameTime
{
    public class TimeScaleHandle : IDisposable
    {
        public event Action<TimeScaleHandle, float> TimeScaleValueChanged;
        private float _timeScaleValue;
        private Action<TimeScaleHandle> _disposeAction;
        private bool _isDisposed;


        public float TimeScaleValue
        {
            get => _timeScaleValue;
            set
            {
                if (Mathf.Approximately(value, _timeScaleValue))
                {
                    return;
                }

                _timeScaleValue = value;
                TimeScaleValueChanged?.Invoke(this, _timeScaleValue);
            }
        }


        public TimeScaleHandle(Action<TimeScaleHandle> disposeAction)
        {
            _timeScaleValue = 1.0f;
            _disposeAction = disposeAction;
        }


        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _isDisposed = true;
            _disposeAction?.Invoke(this);
            _disposeAction = null;
        }
    }
}
