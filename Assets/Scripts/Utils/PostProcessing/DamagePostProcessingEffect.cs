using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;


namespace Honeylab.Utils
{
    public class DamagePostProcessingEffect : MonoBehaviour
    {
        [SerializeField] private Volume _volume;
        [SerializeField] private Ease _easeIn = Ease.Linear;
        [SerializeField] private float _durationIn = 0.1f;
        [SerializeField] private Ease _easeOut = Ease.Linear;
        [SerializeField] private float _durationOut = 0.1f;

        // [SerializeField] private bool _debugPlay;

        private Sequence _sequenceIn;
        private Sequence _sequenceOut;


        private void Start()
        {
            // RunDebugPlayAsync().Forget();
        }


        public void Play()
        {
            if (_sequenceIn != null)
            {
                return;
            }

            DOTween.Kill(_sequenceIn);
            _sequenceIn = null;

            DOTween.Kill(_sequenceOut);
            _sequenceOut = null;

            float weight = _volume.weight;
            float durationIn = _durationIn * (1.0f - _volume.weight);
            _sequenceIn = DOTween.Sequence()
                .SetLink(gameObject)
                .Append(DOTween.To(() => weight, x => weight = x, 1.0f, durationIn)
                    .SetEase(_easeIn)
                    .OnUpdate(() => { _volume.weight = weight; }))
                .OnComplete(() => _sequenceIn = null);

            _sequenceOut = DOTween.Sequence()
                .SetLink(gameObject)
                .AppendInterval(durationIn)
                .Append(DOTween.To(() => weight, x => weight = x, 0.0f, _durationOut)
                    .SetEase(_easeOut)
                    .OnUpdate(() => { _volume.weight = weight; }))
                .OnComplete(() => _sequenceOut = null);
        }


        // private async UniTask RunDebugPlayAsync()
        // {
        //     while (true)
        //     {
        //         await UniTask.WaitUntil(() => _debugPlay);
        //
        //         Play();
        //         _debugPlay = false;
        //     }
        // }
    }
}
