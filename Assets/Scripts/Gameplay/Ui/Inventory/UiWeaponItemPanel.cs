using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;


namespace Honeylab.Gameplay.Ui.Weapons
{
    public class UiWeaponItemPanel : MonoBehaviour
    {
        [SerializeField] private Button _weaponButton;
        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _levelLabel;
        [SerializeField] private TextMeshProUGUI _maxLabel;

        private string _levelLabelFormat;

        public IObservable<Unit> OnWeaponButtonClickAsObservable() => _weaponButton.OnClickAsObservable();

        public void SetIcon(Sprite icon)
        {
            _icon.sprite = icon;
        }


        public void SetLevel(int level)
        {
            _levelLabel.gameObject.SetActive(true);
            _maxLabel.gameObject.SetActive(false);

            if (string.IsNullOrEmpty(_levelLabelFormat))
            {
                _levelLabelFormat = _levelLabel.text;
            }

            string amountText = level.ToString();
            _levelLabel.text = string.Format(_levelLabelFormat, amountText);
        }


        public void SetMaxLevel()
        {
            _levelLabel.gameObject.SetActive(false);
            _maxLabel.gameObject.SetActive(true);
        }
    }
}
