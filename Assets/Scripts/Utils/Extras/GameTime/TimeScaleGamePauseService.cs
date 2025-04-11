using Honeylab.FastFury.Services.GameTime;
using System;
using System.Collections.Generic;


namespace Honeylab.Utils.GameTime
{
    public class TimeScaleGamePauseService : IGamePauseService, IDisposable
    {
        private readonly ITimeScaleService _timeScaleService;

        private readonly Dictionary<object, TimeScaleHandle> _handleByRetainers =
            new Dictionary<object, TimeScaleHandle>();

        private bool _isDisposed;


        public TimeScaleGamePauseService(ITimeScaleService timeScaleService)
        {
            _timeScaleService = timeScaleService;
        }


        public void RetainPause(object retainer)
        {
            ThrowIfDisposed();

            if (_handleByRetainers.ContainsKey(retainer))
            {
                throw new InvalidOperationException($"Can't retain pause multiple times for single {nameof(retainer)}");
            }

            TimeScaleHandle timeScaleHandle = _timeScaleService.CreateTimeScaleHandle();
            _handleByRetainers.Add(retainer, timeScaleHandle);
            timeScaleHandle.TimeScaleValue = 0.0f;
        }


        public void ReleasePause(object retainer)
        {
            if (_isDisposed)
            {
                return;
            }

            if (!_handleByRetainers.TryGetValue(retainer, out TimeScaleHandle timeScaleHandle))
            {
                return;
            }

            _handleByRetainers.Remove(retainer);
            timeScaleHandle.Dispose();
        }


        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _isDisposed = true;

            foreach (var handleKvp in _handleByRetainers)
            {
                handleKvp.Value.Dispose();
            }

            _handleByRetainers.Clear();
        }


        private void ThrowIfDisposed()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException("");
            }
        }
    }
}
