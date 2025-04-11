using Cysharp.Threading.Tasks;
using Honeylab.Utils.Extensions;
using Honeylab.Utils.Pool;
using System;
using System.Collections.Generic;
using System.Threading;
using UniRx;
using UnityEngine;


namespace Honeylab.Utils.Arrows
{
    public abstract class ArrowsPoolBase : IDisposable
    {
        private readonly CancellationTokenSource _disposeCts = new CancellationTokenSource();

        private readonly Dictionary<ArrowView, CompositeDisposable> _arrowPresentations =
            new Dictionary<ArrowView, CompositeDisposable>();

        private readonly IGameObjectPool _gameObjectPool;
        private readonly Transform _defaultParentWhenShowed;
        private readonly ArrowsService _arrowsService;


        protected ArrowsPoolBase(IGameObjectPool gameObjectPool,
            Transform defaultParentWhenShowed,
            ArrowsService arrowsService)
        {
            _gameObjectPool = gameObjectPool;
            _defaultParentWhenShowed = defaultParentWhenShowed;
            _arrowsService = arrowsService;
        }


        public IArrowHandle PopAndShowArrow(Transform parent) => PopAndShowArrow(parent, parent.position);
        public IArrowHandle PopAndShowArrow(Vector3 position) => PopAndShowArrow(_defaultParentWhenShowed, position);
        public IArrowHandle PopAndShowArrowAdvanced(Transform parent, Vector3 position) => PopAndShowArrow(parent, position);

        public async UniTask HideAsync(IArrowHandle handle)
        {
            ArrowHandle internalHandle = (ArrowHandle)handle;
            ArrowView arrow = internalHandle.Arrow;
            ArrowHandle.Release(internalHandle);

            await arrow.HideAsync(_disposeCts.Token);

            _arrowsService.UnregisterArrowView(arrow);
            _arrowPresentations[arrow].Clear();
            _gameObjectPool.Push(arrow.gameObject);
        }

        public void HideImmediately(IArrowHandle handle)
        {
            ArrowHandle internalHandle = (ArrowHandle)handle;
            ArrowView arrow = internalHandle.Arrow;
            ArrowHandle.Release(internalHandle);

            arrow.HideImmediately();

            _arrowsService.UnregisterArrowView(arrow);
            _arrowPresentations[arrow].Clear();
            _gameObjectPool.Push(arrow.gameObject);
        }


        public void Dispose()
        {
            _disposeCts.CancelThenDispose();
            foreach (var presentationKvp in _arrowPresentations)
            {
                presentationKvp.Value.Dispose();
            }
        }


        private ArrowHandle PopAndShowArrow(Transform parent, Vector3 position)
        {
            ArrowView arrowView = _gameObjectPool.PopWithComponent<ArrowView>(true);
            Transform arrowTransform = arrowView.transform;
            arrowTransform.localScale = Vector3.one;
            arrowTransform.parent = parent;
            arrowTransform.position = position;

            arrowView.Show();
            if (!_arrowPresentations.TryGetValue(arrowView, out CompositeDisposable presentation))
            {
                presentation = new CompositeDisposable();
                _arrowPresentations.Add(arrowView, presentation);
            }

            OnPopAndShowArrow(arrowTransform, presentation);

            _arrowsService.RegisterArrowView(arrowView);

            return ArrowHandle.Retain(arrowView);
        }


        protected virtual void OnPopAndShowArrow(Transform arrowTransform, CompositeDisposable disposable) { }
    }
}
