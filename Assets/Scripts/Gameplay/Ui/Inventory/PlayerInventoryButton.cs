using UnityEngine;
using UnityEngine.UI;

namespace Honeylab.Gameplay.Ui
{
    public class PlayerInventoryButton : BaseButton
    {
        [SerializeField] private Image _notificationIcon;

        public void NotificationActive(bool isActive)
        {
            _notificationIcon.gameObject.SetActive(isActive);
        }
    }
}
