using Cysharp.Threading.Tasks;
using Honeylab.Gameplay.Pools;
using Honeylab.Utils.Extensions;
using Honeylab.Utils.Pool;
using System;
using UnityEngine;


namespace Honeylab.Gameplay.Ui
{
    public abstract class PopupViewBase<T> : MonoBehaviour where T : PopupBase
    {
        protected GameplayPoolsService Pools;
        private BillboardPresenterFactory _billboards;

        protected T Popup;
        private IDisposable _disposable;


        public virtual void Init(GameplayPoolsService pools, BillboardPresenterFactory billboards = null)
        {
            if (IsInited())
            {
                return;
            }

            Pools = pools;
            _billboards = billboards;
        }


        protected abstract IGameObjectPool GetPool();


        public T Show()
        {
            if (!IsInited())
            {
                return null;
            }

            IGameObjectPool pool = GetPool();
            Popup = pool.PopWithComponent<T>(false);

            if (_billboards != null)
            {
                _disposable = _billboards.CreateAndRun(Popup.transform);
            }

            Transform popupTransform = Popup.transform;
            popupTransform.parent = transform;
            popupTransform.LocalResetAll();

            Popup.gameObject.SetActive(true);
            Popup.SetShowing(true);

            return Popup;
        }


        public async UniTask HideAsync(bool immediately = false)
        {
            if (Popup == null)
            {
                return;
            }

            Popup.SetShowing(false);
            if (!immediately)
            {
                await Popup.WaitForHideAsync();
            }

            IGameObjectPool pool = GetPool();

            if (Popup?.gameObject != null)
            {
                pool.Push(Popup.gameObject);
            }

            _disposable?.Dispose();
            _disposable = null;

            Popup = null;
        }


        private bool IsInited() => Pools != null;
    }
}
