using UnityEngine;
using UnityEngine.UI;


namespace Honeylab.Gameplay.Ui
{
    public class UiWorldItemInfo : MonoBehaviour
    {
        [SerializeField] private Image _icon;


        public void SetIcon(Sprite icon)
        {
            _icon.sprite = icon;
        }
    }
}
