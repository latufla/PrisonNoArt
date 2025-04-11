using Cysharp.Threading.Tasks;
using Honeylab.Utils.Extensions;
using System;
using System.Threading;
using UniRx;
using UnityEngine;


namespace Honeylab.Gameplay.Player
{
    public class PlayerInputService : IDisposable
    {
        private const int MouseButton = 0;
        private readonly CancellationTokenSource _disposeCts = new CancellationTokenSource();
        private readonly RectTransform _inputRect;
        private readonly Camera _uiCamera;

        private float _power;
        private Vector2 _direction;
        private int _blockCount;
        private Vector2? _startLocalPoint;

        public bool IsBlocked => _blockCount > 0;

        public PlayerInputService(RectTransform inputRect, Camera uiCamera)
        {
            _inputRect = inputRect;
            _uiCamera = uiCamera;
        }


        public async UniTaskVoid RunAsync()
        {
            CancellationToken ct = _disposeCts.Token;
            while (true)
            {
                _startLocalPoint = await WaitForStartLocalPointAsync(ct);

                while (true)
                {
                    await UniTask.Yield(ct);

                    if (!Input.GetMouseButton(MouseButton) ||
                        !TryGetCurrentMouseLocalPoint(out Vector2 currentLocalPoint))
                    {
                        _startLocalPoint = null;
                        break;
                    }

                    Vector2 localPointDelta = currentLocalPoint - _startLocalPoint.Value;
                    _power = Mathf.Clamp01(localPointDelta.magnitude / 1.0f);
                    _direction = _power > 0.0f ? localPointDelta.normalized : Vector2.zero;
                }

                _power = 0.0f;
                _direction = Vector2.zero;
            }
        }


        public float GetPower() => _blockCount <= 0 ? _power : 0.0f;
        public Vector2 GetDirection() => _blockCount <= 0 ? _direction : Vector2.zero;


        public bool TryGetPointerDownLocalPoint(out Vector2 localPoint)
        {
            localPoint = _startLocalPoint.GetValueOrDefault();
            return _startLocalPoint.HasValue && Input.GetMouseButton(MouseButton) && _blockCount <= 0;
        }


        public IDisposable BlockInput()
        {
            _blockCount++;
            return Disposable.Create(() => _blockCount--);
        }


        public void Dispose() => _disposeCts.CancelThenDispose();


        private async UniTask<Vector2> WaitForStartLocalPointAsync(CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            while (true)
            {
                if (Input.GetMouseButton(MouseButton) &&
                    TryGetCurrentMouseLocalPoint(out Vector2 localPoint))
                {
                    return localPoint;
                }

                await UniTask.Yield(ct);
            }
        }


        private bool TryGetCurrentMouseLocalPoint(out Vector2 localPoint)
        {
            Vector2 screenPoint = Input.mousePosition;
            return RectTransformUtility.ScreenPointToLocalPointInRectangle(_inputRect,
                screenPoint,
                _uiCamera,
                out localPoint);
        }
    }
}
