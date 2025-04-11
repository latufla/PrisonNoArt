using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;


namespace Honeylab.Gameplay.Ui
{
    public abstract class ScreenBase : MonoBehaviour
    {
        [SerializeField] private Button _closeButton;
        [SerializeField] private ScreenEntryPointType _entryPointType;

        protected ScreenOpenType _screenOpenType;
        private Subject<ScreenActionInfo> _onScreenAction = new Subject<ScreenActionInfo>();

        public Button CloseButton => _closeButton;
        public abstract string Name { get; }
        public ScreenOpenType ScreenOpenType => _screenOpenType;
        public ScreenEntryPointType ScreenEntryPointType => _entryPointType;
        public IObservable<ScreenActionInfo> OnScreenActionAsObservable() => _onScreenAction.AsObservable();
        public virtual IObservable<Unit> OnCloseButtonClickAsObservable() => _closeButton.OnClickAsObservable();


        public virtual void Show(ScreenOpenType openType)
        {
            _screenOpenType = openType;
            gameObject.SetActive(true);
        }


        public virtual void Hide()
        {
            gameObject.SetActive(false);
        }

        public void CloseButtonInteractable(bool enabled)
        {
            _closeButton.interactable = enabled;
        }

        public void CloseInteract()
        {
            _closeButton.onClick.Invoke();
        }


        public void ScreenAction(string actionName)
        {
            _onScreenAction.OnNext(new ScreenActionInfo(actionName, this));
        }
    }
}
