using Honeylab.Utils.Formatting;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;


namespace Honeylab.Gameplay.Ui.Consumables
{
    public class ConsumablesButton : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private Transform _enabled;
        [SerializeField] private Transform _disabled;
        [SerializeField] private List<Image> _icons;
        [SerializeField] private List<TextMeshProUGUI> _labels;
        [SerializeField] private bool _useLabelFormat;

        private List<string> _labelFormats;

        private bool _isEnabled;


        public IObservable<Unit> OnClickEnabledAsObservable() => _button.OnClickAsObservable().Where(_ => _isEnabled);


        public void SetEnabled(bool isEnabled)
        {
            _isEnabled = isEnabled;

            _enabled.gameObject.SetActive(_isEnabled);
            _disabled.gameObject.SetActive(!_isEnabled);
        }


        public void SetAmount(int amount)
        {
            if (_labels != null)
            {
                string amountText = AcronymedPrint.ToString(amount);
                if (_useLabelFormat)
                {
                    _labelFormats ??= _labels.Select(it => it.text).ToList();
                    int n = _labels.Count;
                    for (int i = 0; i < n; ++i)
                    {
                        TextMeshProUGUI label = _labels[i];
                        label.text = string.Format(_labelFormats[i], amountText);
                    }
                }
                else
                {
                    _labels.ForEach(it => it.text = string.Format(amountText));
                }
            }
        }


        public void SetIcon(Sprite sprite)
        {
            _icons?.ForEach(it => it.sprite = sprite);
        }
    }
}
