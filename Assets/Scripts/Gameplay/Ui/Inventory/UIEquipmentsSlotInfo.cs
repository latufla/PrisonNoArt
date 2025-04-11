using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;


namespace Honeylab.Gameplay.Ui
{
    public class UIEquipmentsSlotInfo : MonoBehaviour
    {
        [SerializeField] private GameObject _defaultIcon;
        [SerializeField] private Image _icon;
        [SerializeField] private GameObject _levelPanel;
        [SerializeField] private TextMeshProUGUI _levelText;
        [SerializeField] private Button _button;
        [SerializeField] private Image _notificationIcon;
        public bool NotificationIsActive { get; private set; }

        public IObservable<Unit> OnButtonClickAsObservable() => _button.OnClickAsObservable();


        public void SetIcon(Sprite icon)
        {
            _icon.sprite = icon;
        }

        public void NotificationInit()
        {
            _notificationIcon.gameObject.SetActive(false);
        }

        public void NotificationActive(bool isActive)
        {
            NotificationIsActive = isActive;
            _notificationIcon.gameObject.SetActive(isActive);
        }


        public void SetAmount(int amount)
        {
            _levelText.text = amount.ToString();
        }


        public void SetActiveIconPanel(bool active)
        {
            _levelPanel.SetActive(active);
            _defaultIcon.SetActive(!active);
            _icon.gameObject.SetActive(active);
        }
    }
}
