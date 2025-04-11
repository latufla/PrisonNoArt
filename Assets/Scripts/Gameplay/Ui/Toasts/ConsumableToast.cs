using Honeylab.Utils.Formatting;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace Honeylab.Gameplay.Ui
{
    public class ConsumableToast : MonoBehaviour
    {
        [SerializeField] private Image _iconImage;
        [SerializeField] private TMP_Text _label;
        [SerializeField] private string _labelFormat = "+{0:ACR}";
        [SerializeField] private ToastAnimation _animation;


        public void SetIconSprite(Sprite sprite) => _iconImage.sprite = sprite;


        public void SetAmount(int amount)
        {
            string text = string.Format(AcronymedPrintCustomFormatter.Instance, _labelFormat, amount);
            _label.text = text;
        }


        public void Show() => _animation.Show();
    }
}
