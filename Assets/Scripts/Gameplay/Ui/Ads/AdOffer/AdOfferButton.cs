using Honeylab.Utils;
using Honeylab.Utils.Formatting;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace Honeylab.Gameplay.Ui.AdOffer
{
    public class AdOfferButton : BaseButton
    {
        [SerializeField] private TimeProgressPanel _timeProgressPanel;
        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _amountText;
        private string _amountTextLabelFormat;

        public TimeProgressPanel TimeProgressPanel => _timeProgressPanel;


        public override void SetActive(bool isEnabled)
        {
            base.SetActive(isEnabled);
            if (TryGetComponent(out ButtonsPlayerInputBlock inputBlock))
            {
                inputBlock.enabled = isEnabled;
            }
        }


        public void SetIcon(Sprite sprite)
        {
            _icon.sprite = sprite;
        }


        public void SetAmount(int amount)
        {
            _amountTextLabelFormat ??= _amountText.text;
            string amountText = AcronymedPrint.ToString(amount);
            _amountText.text = string.Format(_amountTextLabelFormat, amountText);
        }
    }
}
