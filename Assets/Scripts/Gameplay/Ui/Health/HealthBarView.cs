using Cysharp.Threading.Tasks;
using DG.Tweening;
using Honeylab.Utils.Extensions;
using Honeylab.Utils.Formatting;
using MPUIKIT;
using System;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace Honeylab.Gameplay.Ui.Health
{
    public class HealthBarView : MonoBehaviour
    {
        [SerializeField] private Slider _fillSlider;
        [SerializeField] private Slider _processSlider;
        [SerializeField] private MPImage _fillImage;
        [SerializeField] private MPImage _processImage;
        [SerializeField] private float _processDuration = 0.5f;
        [SerializeField] private Color _fillColor = Color.green;
        [SerializeField] private Color _emptyColor = Color.green;
        [SerializeField] private TextMeshProUGUI _healthText;

        //[SerializeField] private Color _processColor = Color.green;
        private CancellationTokenSource _setHealthCts = new CancellationTokenSource();


        public void SetHealthInitial(float health, float maxHealth, bool withAmount = false)
        {
            float healthAmount = health / maxHealth;
            _fillSlider.value = healthAmount;

            _fillImage.color = Color.Lerp(_emptyColor, _fillColor, _fillSlider.value);

            if (_processSlider.value < _fillSlider.value)
            {
                _processSlider.value = _fillSlider.value;
            }

            if (_healthText != null)
            {
                _healthText.gameObject.SetActive(withAmount);

                if (withAmount)
                {
                    _healthText.text = AcronymedPrint.ToString((int)health);
                }
            }
        }

        public void SetHealth(float health, float maxHealth, bool withAnimation = true, bool withAmount = false)
        {
            float healthAmount = health / maxHealth;
            _fillSlider.value = healthAmount;

            _fillImage.color = Color.Lerp(_emptyColor, _fillColor, _fillSlider.value);

            if (_processSlider.value < _fillSlider.value)
            {
                _processSlider.value = _fillSlider.value;
            }

            _setHealthCts?.CancelThenDispose();
            _setHealthCts = new CancellationTokenSource();
            CancellationToken ct = _setHealthCts.Token;

            float endValue = _fillSlider.value;
            if (withAnimation)
            {
                SetProcess(endValue, ct);
            }
            else
            {
                _processSlider.value = endValue;
            }

            if (_healthText != null)
            {
                _healthText.gameObject.SetActive(withAmount);

                if (withAmount)
                {
                    _healthText.text = AcronymedPrint.ToString((int)health);
                }
            }
        }


        private void SetProcess(float endValue, CancellationToken ct)
        {
            //_processImage.color = _processColor;

            DOTween.To(() => _processSlider.value,
                    x => _processSlider.value = x,
                    endValue,
                    _processDuration)
                .SetEase(Ease.InFlash)
                .ToUniTask(cancellationToken: ct)
                .Forget();
        }


        public void Show()
        {
            gameObject.SetActive(true);
        }


        public void Hide()
        {
            gameObject.SetActive(false);
        }


        public void Clear()
        {
            _setHealthCts?.CancelThenDispose();
            _setHealthCts = null;
        }
    }
}
