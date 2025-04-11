using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace Honeylab.Gameplay.Ui.Booster
{
    public class WeaponBoosterFreeButton : BaseButton
    {
        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _label;


        public void SetIcon(Sprite sprite)
        {
            _icon.sprite = sprite;
        }


        public void SetLabel(string text)
        {
            _label.text = text;
        }
    }
}
