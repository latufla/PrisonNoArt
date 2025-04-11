using DG.Tweening;
using System;
using UnityEngine;

namespace Honeylab.Gameplay.Ui
{
    public class SpeechToastAnimation : MonoBehaviour
    {
        [SerializeField] private GameObject _showRoot;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private float _fadeDuration;
        [SerializeField] private Ease _fadeEase;

        public void Show(float time, Action callback)
        {
            DOTween.Kill(this);
            _canvasGroup.alpha = 0.0f;
            _showRoot.SetActive(true);

            DOTween.Sequence()
                .SetId(this)
                .AppendInterval(time)
                .Insert(0,_canvasGroup.DOFade(1.0f, _fadeDuration)
                    .SetLink(_canvasGroup.gameObject)
                    .SetEase(_fadeEase))
                .Insert(time - _fadeDuration,_canvasGroup.DOFade(0.0f, _fadeDuration)
                    .SetLink(_canvasGroup.gameObject)
                    .SetEase(_fadeEase))
                .AppendCallback(() =>
                {
                    _showRoot.SetActive(false);
                    callback.Invoke();
                });
        }
    }
}
