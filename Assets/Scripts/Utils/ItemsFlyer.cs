using Cysharp.Threading.Tasks;
using DG.Tweening;
using Honeylab.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UniRx;
using UnityEngine;
using UnityEngine.UI;


namespace Honeylab.Utils
{
    public class ItemsFlyer : MonoBehaviour
    {
        [SerializeField] private Transform _root;
        [SerializeField] private float _itemScaleDuration = 0.4f;
        [SerializeField] private float _itemFlyDelay = 0.2f;
        [SerializeField] private float _itemFlyDuration = 0.4f;
        [SerializeField] private float _itemScaleMultiplier = 0.3f;

        private List<Image> _icons;
        private readonly ISubject<Unit> _flyEndSubject = new Subject<Unit>();


        public IObservable<Unit> OnItemFlyEndAsObservalbe() => _flyEndSubject.AsObservable();


        public void Init()
        {
            _icons = _root.GetComponentsInChildren<Image>(true).ToList();
            _icons.ForEach(it => it.gameObject.SetActive(false));
        }


        public async UniTask RunAsync(Sprite icon, int amount, Vector3 toPosition, CancellationToken ct)
        {
            int m = _icons.Count;
            for (int i = 0; i < amount && i < m; ++i)
            {
                Image image = _icons[i];
                InitImage(image, icon);
                image.gameObject.SetActive(true);

                Vector3 position = image.transform.position;
                Vector3 localScale = image.transform.localScale;
                image.transform.localScale = Vector3.zero;
                DOTween.Sequence()
                    .SetLink(image.gameObject)
                    .SetId(this)
                    .Append(image.transform.DOScale(localScale, _itemScaleDuration))
                    .AppendInterval(_itemFlyDelay * i)
                    .Append(image.transform.DOMove(toPosition, _itemFlyDuration))
                    .OnComplete(() => { _flyEndSubject.OnNext(); })
                    .OnKill(() => { ClearImage(image, position, localScale); })
                    .ToUniTask(cancellationToken: ct)
                    .Forget();
            }

            await UniTask.Delay(TimeSpan.FromSeconds(amount * (0.4f + 0.2f + _itemFlyDuration)), cancellationToken: ct);
        }


        public void Clear()
        {
            DOTween.Kill(this);
        }


        private void InitImage(Image image, Sprite icon)
        {
            image.sprite = icon;
            image.transform.localScale = Vector3.one * _itemScaleMultiplier;
        }


        private void ClearImage(Image image, Vector3 position, Vector3 localScale)
        {
            if (image != null)
            {
                image.gameObject.SetActive(false);
                image.transform.position = position;
                image.transform.localScale = localScale;
            }
        }
    }
}
