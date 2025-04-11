using Cysharp.Threading.Tasks;
using Honeylab.Utils.Extensions;
using System;
using System.Threading;
using UnityEngine;


namespace Honeylab.Utils.GameInput
{
    public class GameInputPresenter : IDisposable
    {
        private readonly CancellationTokenSource _disposeCts = new CancellationTokenSource();
        private readonly GameInputView _view;
        private readonly GameInputService _input;

        private bool _shouldHide;


        public GameInputPresenter(GameInputView view, GameInputService input)
        {
            _view = view;
            _input = input;
        }


        public void Init()
        {
            _view.Init();
            _shouldHide = true;
        }


        public async UniTaskVoid RunAsync()
        {
            CancellationToken ct = _disposeCts.Token;
            while (true)
            {
                if (_input.TryGetPointerDownLocalPoint(out Vector2 pointerDownLocalPoint))
                {
                    _shouldHide = false;

                    Vector2 inputValue = _input.GetPower() * _input.GetDirection();
                    _view.PresentInput(pointerDownLocalPoint, inputValue);
                }
                else if (!_shouldHide)
                {
                    _shouldHide = true;
                    _view.Fade();
                }

                await UniTask.Yield(ct);
            }
        }


        public void Dispose() => _disposeCts.CancelThenDispose();
    }
}
