using DG.Tweening;
using MPUIKIT;
using UnityEngine;


namespace Honeylab.Utils
{
    public class JoystickInputView : MonoBehaviour
    {
        [SerializeField] private RectTransform _root;
        [SerializeField] private MPImageBasic _backgroundImage;
        [SerializeField] private MPImageBasic _stickImage;
        [SerializeField] private float _fadeDuration;

        private Color _backgroundImageInitialColor;
        private Color _stickImageInitialColor;
        private float _multiplier;


        public void Init()
        {
            _backgroundImageInitialColor = _backgroundImage.color;
            _stickImageInitialColor = _stickImage.color;

            Rect backgroundRect = _backgroundImage.rectTransform.rect;
            float backgroundMinDimension = Mathf.Min(backgroundRect.width, backgroundRect.height) / 2.0f;

            Rect stickRect = _stickImage.rectTransform.rect;
            float stickMinHalfDimension = Mathf.Min(stickRect.width, stickRect.height) / 2.0f;

            _multiplier = backgroundMinDimension - stickMinHalfDimension;

            SetActive(false);
        }


        public void PresentInput(Vector2 rootAnchoredPosition, Vector2 inputValue)
        {
            DOTween.Kill(this);
            SetActive(true);

            _backgroundImage.color = _backgroundImageInitialColor;
            _stickImage.color = _stickImageInitialColor;

            _root.anchoredPosition = rootAnchoredPosition;

            if (inputValue.sqrMagnitude > 0.0f)
            {
                _stickImage.rectTransform.anchoredPosition = inputValue * _multiplier;
            }
        }


        public void Fade()
        {
            if (DOTween.IsTweening(this))
            {
                return;
            }

            DOTween.Sequence()
                .SetId(this)
                .SetUpdate(true)
                .SetLink(gameObject)
                .Append(_backgroundImage.DOFade(0.0f, _fadeDuration))
                .Join(_stickImage.DOFade(0.0f, _fadeDuration))
                .AppendCallback(() => SetActive(false));
        }


        private void SetActive(bool isActive) => _root.gameObject.SetActive(isActive);
    }
}
