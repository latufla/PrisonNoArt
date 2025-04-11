using DG.Tweening;
using UnityEngine;


namespace Honeylab.MoneyGarden.Ui
{
    public class LoadingSpinnerView : MonoBehaviour
    {
        [SerializeField] private Transform _rotateRoot;

        private bool _shouldTween;


        public void SetTweening(bool isTweening)
        {
            if (_shouldTween == isTweening)
            {
                return;
            }

            _shouldTween = isTweening;
            if (isTweening)
            {
                ResetRotation();

                bool isPaused = false;
                _rotateRoot.DOLocalRotate(360.0f * Vector3.forward, 1.0f, RotateMode.LocalAxisAdd)
                    .SetLink(_rotateRoot.gameObject, LinkBehaviour.PauseOnDisablePlayOnEnable)
                    .SetEase(Ease.Linear)
                    .OnPause(() => isPaused = true)
                    .OnPlay(() =>
                    {
                        if (isPaused)
                        {
                            ResetRotation();
                        }

                        isPaused = false;
                    })
                    .SetLoops(-1);
            }
            else
            {
                _rotateRoot.DOKill();
            }
        }


        private void ResetRotation() => _rotateRoot.localRotation = Quaternion.identity;
    }
}
