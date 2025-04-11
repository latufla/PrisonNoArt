using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Honeylab.Utils.Formatting;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace Honeylab.Gameplay.Ui
{
    public class ConsumableCounterView : MonoBehaviour
    {
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private Image _iconImage;
        [SerializeField] private TMP_Text[] _labels;
        [SerializeField] private float _changeDuration = 1.0f;

        private int _amount;
        private TweenerCore<float, float, FloatOptions> _tween;

        public void SetIcon(Sprite iconSprite) => _iconImage.sprite = iconSprite;


        public void SetVisible(bool isVisible)
        {
            gameObject.SetActive(isVisible);
            if (!isVisible)
            {
                SetAsLastSibling();
            }
        }


        public bool IsVisible() => gameObject.activeInHierarchy;


        public void SetAsFirstSibling()
        {
            var screenConsumableTransform = transform;
            screenConsumableTransform.SetAsFirstSibling();
            RectTransform rect = screenConsumableTransform.parent as RectTransform;
            LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
        }


        public void SetAsLastSibling()
        {
            var screenConsumableTransform = transform;
            screenConsumableTransform.SetAsLastSibling();
            RectTransform rect = screenConsumableTransform.parent as RectTransform;
            LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
        }


        public void SetSiblingIndexIfNotVisible(int index)
        {
            if (IsVisible())
            {
                return;
            }

            var screenConsumableTransform = transform;
            screenConsumableTransform.SetSiblingIndex(index);
            RectTransform rect = screenConsumableTransform.parent as RectTransform;
            LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
        }


        public RectTransform GetRectTransform() => _rectTransform;

        public RectTransform GetIconTransform() => _iconImage.rectTransform;


        public void ChangeAmount(int delta, bool animated = false)
        {
            var newAmount = _amount += delta;
            SetAmount(newAmount, animated);
        }


        public void SetAmount(int amount, bool animated = false)
        {
            if (_tween != null)
            {
                DOTween.Kill(_tween);
                _tween = null;
            }

            if (animated)
            {
                float currentAmount = _amount;
                _tween = DOTween.To(() => currentAmount, x => currentAmount = x, amount, _changeDuration)
                    .OnUpdate(() => { SetAmountText(Mathf.CeilToInt(currentAmount)); });

                PlayAnimCounter();
            }
            else
            {
                SetAmountText(amount);
            }

            _amount = amount;
        }


        protected void OnDestroy()
        {
            if (_tween != null)
            {
                DOTween.Kill(_tween);
                _tween = null;
            }
        }


        private void SetAmountText(int amount)
        {
            string text = AcronymedPrint.ToString(amount);
            foreach (TMP_Text label in _labels)
            {
                label.text = text;
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
