using Honeylab.Utils.Formatting;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace Honeylab.Gameplay.Buildings
{
    public class UpgradeConsumablesAmountPanel : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _amountLabel;

        private string _amountLabelFormat;


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
    }
}
