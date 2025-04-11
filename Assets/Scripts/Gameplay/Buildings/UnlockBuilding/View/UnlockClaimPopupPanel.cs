using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;


namespace Honeylab.Gameplay.Buildings.View
{
    public class UnlockClaimPopupPanel : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private Button _claimUnlockButtonEnabled;


        public IObservable<Unit> OnClaimUnlockButtonClickAsObservable() =>
            _claimUnlockButtonEnabled.OnClickAsObservable();


        public void Show(UnlockBuildingFlow flow, UnlockBuildingView view)
        {
            gameObject.SetActive(true);

            UnlockBuildingInteractable interactable = flow.Get<UnlockBuildingInteractable>();
            bool isClaim = interactable != null && interactable.HasAgent();
            _claimUnlockButtonEnabled.gameObject.SetActive(isClaim);

            _icon.sprite = view.ResultIconSprite;

            LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
        }


        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
