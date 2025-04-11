using Honeylab.Gameplay.Ui;
using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;


namespace Honeylab.Gameplay.Buildings.View
{
    public class UnlockBuildingPopup : PopupBase
    {
        [SerializeField] private GameObject _titleContent;
        [SerializeField] private TextMeshProUGUI _titleLabel;
        [SerializeField] private RectTransform _layout;
        [SerializeField] private UnlockConsumablesPopupPanel _unlockConsumablesPanel;
        [SerializeField] private UnlockProgressPanel _unlockProgressPanel;
        [SerializeField] private UnlockClaimPopupPanel _unlockClaimPanel;
        [SerializeField] private Button _emptyCompleteButton;

        public UnlockConsumablesPopupPanel UnlockConsumablesPanel => _unlockConsumablesPanel;
        public UnlockProgressPanel UnlockProgressPanel => _unlockProgressPanel;
        public UnlockClaimPopupPanel UnlockClaimPanel => _unlockClaimPanel;
        public Button EmptyCompleteButton => _emptyCompleteButton;
        public RectTransform Layout => _layout;

        public IObservable<Unit> OnEmptyCompleteButtonClickAsObservable() => _emptyCompleteButton.OnClickAsObservable();

        public void SetTitle(string title)
        {
            bool active = !string.IsNullOrEmpty(title);
            _titleContent.SetActive(active);
            _titleLabel.gameObject.SetActive(active);
            _titleLabel.text = title;
        }
    }
}
