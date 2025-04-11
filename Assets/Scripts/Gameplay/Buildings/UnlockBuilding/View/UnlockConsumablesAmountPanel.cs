using Honeylab.Utils.Formatting;
using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;


namespace Honeylab.Gameplay.Buildings.View
{
    public class UnlockConsumablesAmountPanel : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _nameLabel;
        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _amountLabel;
        [SerializeField] private Button _useButtonEnabled;
        [SerializeField] private Transform _useButtonDisabled;
        [SerializeField] private Transform _completePanel;

        private string _amountLabelFormat;


        public IObservable<Unit> OnUseButtonClickAsObserver() => _useButtonEnabled.OnClickAsObservable();


        public void SetName(string nameStr)
        {
            _nameLabel.text = nameStr;
        }


        public void SetIcon(Sprite icon)
        {
            _icon.sprite = icon;
        }


        public void SetAmount(int amount, int maxAmount)
        {
            if (string.IsNullOrEmpty(_amountLabelFormat))
            {
                _amountLabelFormat = _amountLabel.text;
            }

            string amountText = AcronymedPrint.ToString(amount);
            string maxAmountText = AcronymedPrint.ToString(maxAmount);
            _amountLabel.text = string.Format(_amountLabelFormat, amountText, maxAmountText);
        }


        public void SetUseButtonEnabled(bool isEnabled)
        {
            _useButtonEnabled.gameObject.SetActive(isEnabled);
            _useButtonDisabled.gameObject.SetActive(!isEnabled);
        }


        public void SetComplete(bool isEnabled)
        {
            _useButtonEnabled.gameObject.SetActive(false);
            _useButtonDisabled.gameObject.SetActive(false);

            _completePanel.gameObject.SetActive(isEnabled);
        }
    }
}
