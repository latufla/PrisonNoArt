using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;


namespace Honeylab.Gameplay.Ui
{
    public class UiConsumableItemInfo : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private Button _button;
        [SerializeField] private TextMeshProUGUI _amountText;
        [SerializeField] private GameObject _amountStorage;

        public IObservable<Unit> OnClick => _button.onClick.AsObservable();

        private string _name;

        private Color _defaultColor;

        public string Name => _name;


        public void SetIcon(Sprite icon)
        {
            _icon.sprite = icon;
        }


        public void SetName(string name)
        {
            _name = name;
        }


        public void SetAmount(int amount)
        {
            _amountText.text = amount.ToString();
        }


        public void SetAmount(string amount)
        {
            _amountText.text = amount;
        }


        public void AmountActive(bool active)
        {
            _amountStorage.SetActive(active);
        }


        public void SetColorText(Color color)
        {
            _defaultColor = _amountText.color;
            _amountText.color = color;
        }


        public void ResetColorText()
        {
            if (_defaultColor == default)
            {
                return;
            }
            _amountText.color = _defaultColor;
        }
    }
}
