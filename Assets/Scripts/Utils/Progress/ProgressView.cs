using DG.Tweening;
using MPUIKIT;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Honeylab.Utils
{
    public class ProgressView : MonoBehaviour
    {
        [SerializeField] private Transform _amountPanel;
        [SerializeField] private TextMeshProUGUI _amountLabel;
        [SerializeField] private TextMeshProUGUI _fullLabel;
        [SerializeField] private MPImage _progressFillImage;
        [SerializeField] private MPImage _lateProgressFillImage;
        [SerializeField] private Image _icon;

        private Sequence _sequence;
        private string _amountLabelFormat;

        public void SetImage(Sprite sprite) => _icon.sprite = sprite;

        public void UpdateView(int amount, int maxAmount)
        {
            SetProgress(amount, maxAmount);
        }


        private void UpdateAmountPanel(int amount, int maxAmount, bool isActive)
        {
            if (_amountPanel != null && _amountLabel != null)
            {
                _amountLabelFormat ??= _amountLabel.text;
                _amountLabel.text = string.Format(_amountLabelFormat, amount, maxAmount);
                _amountPanel.gameObject.SetActive(isActive);
            }
        }


        private void UpdateFullLabel(int amount, int maxAmount)
        {
            if (_fullLabel != null)
            {
                bool isFull = amount >= maxAmount;
                _fullLabel.gameObject.SetActive(isFull);

                UpdateAmountPanel(amount, maxAmount, !isFull);
            }
        }


        private void SetProgress(int amount, int maxAmount)
        {
            UpdateAmountPanel(amount, maxAmount, true);
            UpdateFullLabel(amount, maxAmount);

            float ratio = Mathf.Clamp01(amount / (float)maxAmount);
            if (_progressFillImage != null)
            {
                _progressFillImage.fillAmount = ratio;
            }

            if (_lateProgressFillImage)
            {
                _sequence?.Kill();
                _sequence = DOTween.Sequence()
                    .SetLink(_lateProgressFillImage.gameObject);

                float oldAmount = _lateProgressFillImage.fillAmount;
                if (ratio < oldAmount)
                {
                    _lateProgressFillImage.fillAmount = 0;
                }
                _sequence.Append(DOTween.To(() => _lateProgressFillImage.fillAmount,
                        x => _lateProgressFillImage.fillAmount = x,
                        ratio,
                        1.2f)
                    .SetEase(Ease.InFlash));
            }
        }
    }
}
