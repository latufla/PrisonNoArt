using DG.Tweening;
using UnityEngine;


namespace Honeylab.Utils
{
    public class ScaleInteractionView : MonoBehaviour
    {
        [SerializeField] private Transform _scaleRoot;
        [SerializeField] [Min(0.0f)] private float _scaleValue;
        [SerializeField] [Min(0.0f)] private float _duration;
        [SerializeField] private Ease _enableEase;
        [SerializeField] private Ease _disableEase;

        private Vector3 _scaleRootLocalScaleBackup;


        private void Awake()
        {
            _scaleRootLocalScaleBackup = _scaleRoot.localScale;
        }


        public void SetInteracting(bool isInteracting)
        {
            Vector3 newScale = isInteracting ?
                _scaleValue * _scaleRootLocalScaleBackup :
                _scaleRootLocalScaleBackup;
            Ease ease = isInteracting ? _enableEase : _disableEase;

            DOTween.Kill(this);
            DOTween.Sequence()
                .SetId(this)
                .Append(_scaleRoot.DOScale(newScale, _duration).SetEase(ease));
        }


        private void OnDestroy()
        {
            DOTween.Kill(this);
        }
    }
}
