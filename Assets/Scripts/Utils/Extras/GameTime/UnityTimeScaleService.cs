using Honeylab.FastFury.Services.GameTime;
using System;
using System.Collections.Generic;
using UnityEngine;


namespace Honeylab.Utils.GameTime
{
    public class UnityTimeScaleService : ITimeScaleService, IDisposable
    {
        private readonly HashSet<TimeScaleHandle> _timeScaleHandles = new HashSet<TimeScaleHandle>();
        private readonly bool _shouldScaleFixedDeltaTime;
        private readonly float _initialFixedDeltaTime;
        private bool _isDisposed;


        public UnityTimeScaleService(bool shouldScaleFixedDeltaTime)
        {
            _shouldScaleFixedDeltaTime = shouldScaleFixedDeltaTime;
            _initialFixedDeltaTime = Time.fixedDeltaTime;
        }


        public TimeScaleHandle CreateTimeScaleHandle()
        {
            ThrowIfDisposed();

            TimeScaleHandle newHandle = new TimeScaleHandle(TimeScaleHandleDisposed);
            _timeScaleHandles.Add(newHandle);
            RefreshTimeScaleIfNeeded();
            newHandle.TimeScaleValueChanged += TimeScaleHandle_OnTimeScaleValueChanged;
            return newHandle;
        }


        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _isDisposed = true;

            foreach (TimeScaleHandle handle in _timeScaleHandles)
            {
                handle.TimeScaleValueChanged -= TimeScaleHandle_OnTimeScaleValueChanged;
            }

            _timeScaleHandles.Clear();

            Time.timeScale = 1.0f;
            if (_shouldScaleFixedDeltaTime)
            {
                Time.fixedDeltaTime = _initialFixedDeltaTime;
            }
        }


        private void RefreshTimeScaleIfNeeded()
        {
            if (_isDisposed)
            {
                return;
            }

            float timeScale = 1.0f;
            foreach (TimeScaleHandle handle in _timeScaleHandles)
            {
                timeScale *= handle.TimeScaleValue;
            }

            Time.timeScale = timeScale;

            if (_shouldScaleFixedDeltaTime)
            {
                Time.fixedDeltaTime = _initialFixedDeltaTime * timeScale;
            }
        }


        private void TimeScaleHandleDisposed(TimeScaleHandle handle)
        {
            if (_isDisposed)
            {
                return;
            }

            _timeScaleHandles.Remove(handle);
            RefreshTimeScaleIfNeeded();
        }


        private void ThrowIfDisposed()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException("");
            }
        }


        private void TimeScaleHandle_OnTimeScaleValueChanged(TimeScaleHandle handle, float timeScaleValue) =>
            RefreshTimeScaleIfNeeded();
    }
}
