using Cysharp.Threading.Tasks;
using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;


namespace Honeylab.Gameplay.Ui
{
    public class ConfirmationPopup : MonoBehaviour
    {
        [SerializeField] private PopupAnimation _animation;
        [SerializeField] private Button _confirmButton;

        public IObservable<Unit> OnConfirmButtonClickAsObservable() => _confirmButton.OnClickAsObservable();
        public void SetShowing(bool isShowing) => _animation.SetShowing(isShowing);
        public UniTask WaitForHideAsync() => _animation.WaitForHideAsync();
    }
}
