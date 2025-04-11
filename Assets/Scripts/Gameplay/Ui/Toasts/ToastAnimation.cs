using DG.Tweening;
using UnityEngine;


namespace Honeylab.Gameplay.Ui
{
    public class ToastAnimation : MonoBehaviour
    {
        [SerializeField] private GameObject _showRoot;
        [SerializeField] [Min(0.0f)] private float _moveDuration;
        [SerializeField] private RectTransform _moveRoot;
        [SerializeField] [Min(0.0f)] private float _moveHeight;
        [SerializeField] private Ease _moveEase;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private float _fadeDuration;
        [SerializeField] private Ease _fadeEase;

        private Vector3 _moveRootLocalPositionBackup;


        private void Awake()
        {
            _moveRootLocalPositionBackup = _moveRoot.localPosition;
            _showRoot.SetActive(false);
        }


        public void Show()
        {
            DOTween.Kill(this);
            _canvasGroup.alpha = 1.0f;
            _moveRoot.localPosition = _moveRootLocalPositionBackup;
            _showRoot.SetActive(true);

            DOTween.Sequence()
                .SetId(this)
                .Append(_moveRoot.DOLocalMoveY(_moveHeight, _moveDuration)
                    .SetLink(_moveRoot.gameObject)
                    .SetRelative()
                    .SetEase(_moveEase))
                .Insert(_moveDuration - _fadeDuration,
                    _canvasGroup.DOFade(0.0f, _fadeDuration)
                        .SetLink(_canvasGroup.gameObject)
                        .SetEase(_fadeEase))
                .AppendCallback(() => _showRoot.SetActive(false));
        }
    }
}
