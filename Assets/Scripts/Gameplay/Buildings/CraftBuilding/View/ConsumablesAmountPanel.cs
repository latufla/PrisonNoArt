using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Honeylab.Utils.Data;
using Honeylab.Utils.Formatting;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace Honeylab.Gameplay.Buildings
{
    public class ConsumablesAmountPanel : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _amountLabel;
        [SerializeField] private TextMeshProUGUI _descriptionLabel;
        [SerializeField] private Transform _donePanel;
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private float _changeDuration = 1.0f;
        private Color _defaultColor = Color.white;

        private ScriptableId _id;
        private string _amountLabelFormat;

        private TweenerCore<float, float, FloatOptions> _tween;
        private int _amount;


        public void SetId(ScriptableId id)
        {
            _id = id;
        }


        public ScriptableId GetId() => _id;


        public void SetIcon(Sprite icon)
        {
            _icon.sprite = icon;
        }


        public void SetAmount(int amount, int maxAmount)
        {
            if (string.IsNullOrEmpty(_amountLabelFormat))
            {
                _amountLabelFormat = _amountLabel.text;
            }

            string amountText = AcronymedPrint.ToString(amount);
            string maxAmountText = AcronymedPrint.ToString(maxAmount);
            _amountLabel.text = string.Format(_amountLabelFormat, amountText, maxAmountText);
        }


        public void SetAmount(int maxAmount)
        {
            if (string.IsNullOrEmpty(_amountLabelFormat))
            {
                _amountLabelFormat = _amountLabel.text;
            }

            string maxAmountText = AcronymedPrint.ToString(maxAmount);
            _amountLabel.text = maxAmountText;
        }


        public void SetDescription(string description)
        {
            if (_descriptionLabel == null)
            {
                return;
            }

            _descriptionLabel.text = description;
        }


        public void SetDone(bool isDone)
        {
            if (_donePanel == null)
            {
                return;
            }

            _donePanel.gameObject.SetActive(isDone);
            _amountLabel.gameObject.SetActive(!isDone);
        }


        public void SetAmountAnimated(int amount, bool animated = false)
        {
            if (_tween != null)
            {
                DOTween.Kill(_tween);
                _tween = null;
            }

            if (animated)
            {
                if (amount > _amount)
                {
                    float currentAmount = _amount;
                    _tween = DOTween.To(() => currentAmount, x => currentAmount = x, amount, _changeDuration)
                        .OnUpdate(() => { SetAmount(Mathf.CeilToInt(currentAmount)); });
                }
                else
                {
                    SetAmount(amount);
                }

                PlayAnimCounter();
            }
            else
            {
                SetAmount(amount);
            }

            _amount = amount;
        }


        public void SetAmountColor(Color color)
        {
            _amountLabel.color = color;
        }


        public void ResetAmountColor()
        {
            _amountLabel.color = _defaultColor;
        }


        protected void OnDestroy()
        {
            if (_tween != null)
            {
                DOTween.Kill(_tween);
                _tween = null;
            }
        }


        private void PlayAnimCounter()
        {
            DOTween.Kill(this);

            DOTween.Sequence()
                .SetId(this)
                .SetLink(_rectTransform.gameObject)
                .Append(_rectTransform.DOScale(new Vector3(1.15f, 1.15f, 1.15f), 0.1f))
                .Append(_rectTransform.DOScale(Vector3.one, 0.1f));
        }
    }
}
