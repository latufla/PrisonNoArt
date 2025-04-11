using Cysharp.Threading.Tasks;
using Honeylab.Utils.Extensions;
using System;
using System.Threading;
using UnityEngine;


namespace Honeylab.Gameplay.Player
{
    public class PlayerInputPresenter : IDisposable
    {
        private readonly CancellationTokenSource _disposeCts = new CancellationTokenSource();
        private readonly PlayerInputView _view;
        private readonly PlayerInputService _input;

        private bool _shouldHide;


        public PlayerInputPresenter(PlayerInputView view, PlayerInputService input)
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
