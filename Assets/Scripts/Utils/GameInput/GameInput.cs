using UnityEngine;


namespace Honeylab.Utils.GameInput
{
    public class GameInput : MonoBehaviour
    {
        private GameInputService _inputService;
        private GameInputPresenter _inputPresenter;
        private GameInputView _view;


        public void Init(RectTransform inputRect, Camera uiCamera, GameInputView view)
        {
            _view = view;

            _inputService = new GameInputService(inputRect, uiCamera, 1.0f);
            _inputPresenter = new GameInputPresenter(_view, _inputService);
            _inputPresenter.Init();
        }


        public void Run()
        {
            _inputService.RunAsync().Forget();
            _inputPresenter.RunAsync().Forget();
        }


        public void Clear()
        {
            _inputPresenter?.Dispose();
            _inputPresenter = null;

            _inputService?.Dispose();
            _inputService = null;
        }


        public Vector2 GetDirection() => _inputService?.GetDirection() ?? Vector2.zero;
    }
}
