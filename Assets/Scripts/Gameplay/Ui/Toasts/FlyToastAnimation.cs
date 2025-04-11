using DG.Tweening;
using Honeylab.Consumables;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using Random = UnityEngine.Random;


namespace Honeylab.Gameplay.Ui
{
    public class FlyToastAnimation : MonoBehaviour
    {
        [SerializeField] private GameObject _showRoot;

        [SerializeField] [Min(0.0f)] private float _moveDistance = 3;
        [SerializeField] [Min(0.0f)] private float _moveDuration;
        [SerializeField] [Min(0.0f)] private float _jumpPower = 2.0f;
        [SerializeField] private RectTransform _moveRoot;
        [SerializeField] private Ease _moveEase;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] [Min(0.0f)] private float _pauseDelay = 0.5f;
        [SerializeField] [Min(0.0f)] private float _outMoveDuration;
        [SerializeField] private Ease _outMoveEase;
        [SerializeField] private RectTransform _icon;
        private Vector2 _oldScaleRect;
        private Vector3 _moveRootLocalPositionBackup;


        private void Awake()
        {
            _moveRootLocalPositionBackup = _moveRoot.localPosition;
            _showRoot.SetActive(false);
            _oldScaleRect = _icon.sizeDelta;
        }


        public void Show(ConsumableCounterView screenConsumable,
            Transform parent,
            Vector3 toastEndPosition,
            Action callback)
        {
            RefreshIcon(_moveRoot);
            _canvasGroup.alpha = 1.0f;
            _moveRoot.localPosition = _moveRootLocalPositionBackup;
            _showRoot.SetActive(true);

            float minMovePositionX = -_moveDistance;
            float maxMovePositionX = _moveDistance;

            float minMovePositionY = -_moveDistance;
            float maxMovePositionY = _moveDistance;

            float movePositionX = Random.Range(minMovePositionX, maxMovePositionX);
            float movePositionY = Random.Range(minMovePositionY, maxMovePositionY);

            Vector3 rootPos = _moveRoot.position;

            Vector3 movePosition = new Vector3(toastEndPosition.x + movePositionX,
                rootPos.y,
                toastEndPosition.z + movePositionY);


            DOTween.Sequence()
                .SetId(this)
                .SetLink(_moveRoot.gameObject)
                .Append(_moveRoot.DOJump(movePosition, _jumpPower, 1, _moveDuration)
                    .SetEase(_moveEase))
                .Append(_moveRoot.DOJump(movePosition, _jumpPower / 6, 1, _moveDuration / 4)
                    .SetEase(_moveEase))
                .Insert(0.2f,
                    _icon.DOScale(Vector3.one / 1.2f, _outMoveDuration)
                        .SetEase(_outMoveEase))
                .AppendInterval(_pauseDelay)
                .AppendCallback(() => ChangeCanvas((RectTransform)parent))
                .Append(_icon.DOJump(screenConsumable.GetIconTransform().position, 0.5f, 1, _outMoveDuration)
                    .SetEase(_outMoveEase))
                .Insert(_moveDuration + _pauseDelay + (_outMoveDuration / 2),
                    _icon.DOScale(Vector3.one / 1.2f, _outMoveDuration / 2)
                        .SetEase(_outMoveEase))
                .AppendCallback(() =>
                {
                    _showRoot.SetActive(false);
                    RefreshIcon(_moveRoot);
                })
                .OnKill(() =>
                {
                    callback?.Invoke();
                });
        }


        public void Show(Transform target,
            Transform parent,
            Vector3 toastEndPosition,
            Action callback)
        {
            RefreshIcon(_moveRoot);
            _canvasGroup.alpha = 1.0f;
            _moveRoot.localPosition = _moveRootLocalPositionBackup;
            _showRoot.SetActive(true);

            float minMovePositionX = -_moveDistance;
            float maxMovePositionX = _moveDistance;

            float minMovePositionY = -_moveDistance;
            float maxMovePositionY = _moveDistance;

            float movePositionX = Random.Range(minMovePositionX, maxMovePositionX);
            float movePositionY = Random.Range(minMovePositionY, maxMovePositionY);

            Vector3 rootPos = _moveRoot.position;

            Vector3 movePosition = new(toastEndPosition.x + movePositionX,
                rootPos.y,
                toastEndPosition.z + movePositionY);

            DOTween.Sequence()
                .SetId(this)
                .SetLink(_moveRoot.gameObject)
                .Append(_moveRoot.DOJump(movePosition, _jumpPower, 1, _moveDuration)
                    .SetEase(_moveEase))
                .Append(_moveRoot.DOJump(movePosition, _jumpPower / 6, 1, _moveDuration / 4)
                    .SetEase(_moveEase))
                .Insert(0.2f,
                    _icon.DOScale(Vector3.one / 1.2f, _outMoveDuration)
                        .SetEase(_outMoveEase))
                .AppendInterval(_pauseDelay)
                .AppendCallback(() => ChangeCanvas((RectTransform)parent))
                .Append(_icon.DOJump(target.position, 0.5f, 1, _outMoveDuration)
                    .SetEase(_outMoveEase))
                .Insert(_moveDuration + _pauseDelay + (_outMoveDuration / 2),
                    _icon.DOScale(Vector3.one / 1.2f, _outMoveDuration / 2)
                        .SetEase(_outMoveEase))
                .AppendCallback(() =>
                {
                    _showRoot.SetActive(false);
                    RefreshIcon(_moveRoot);
                })
                .OnKill(callback.Invoke);
        }


        private void ChangeCanvas(RectTransform parent)
        {
            var canvas = parent.GetParentCanvas();
            Vector3 screenPos = Camera.main.WorldToScreenPoint(_icon.position);
            Vector2 screenPos2D = new(screenPos.x, screenPos.y);
            RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)canvas.transform,
                screenPos2D,
                canvas.worldCamera,
                out Vector2 endPos);

            _icon.SetParent(canvas.transform, true);
            _icon.localScale = Vector3.one;
            _icon.sizeDelta = new Vector2(100, 100);
            _icon.localEulerAngles = Vector3.zero;
            _icon.localPosition = endPos;
        }


        private void RefreshIcon(Transform parent)
        {
            _icon.SetParent(parent, false);
            _icon.localScale = Vector3.one;
            _icon.sizeDelta = _oldScaleRect;
            _icon.localPosition = Vector3.zero;
        }
    }
}
