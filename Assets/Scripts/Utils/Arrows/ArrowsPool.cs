using Honeylab.Gameplay.Ui;
using Honeylab.Utils.Pool;
using UniRx;
using UnityEngine;


namespace Honeylab.Utils.Arrows
{
    public class ArrowsPool : ArrowsPoolBase
    {
        private readonly BillboardPresenterFactory _billboardPresenterFactory;


        public ArrowsPool(IGameObjectPool gameObjectPool,
            Transform defaultParentWhenShowed,
            BillboardPresenterFactory billboardPresenterFactory,
            ArrowsService arrowsService) : base(gameObjectPool, defaultParentWhenShowed, arrowsService)
        {
            _billboardPresenterFactory = billboardPresenterFactory;
        }


        protected override void OnPopAndShowArrow(Transform arrowTransform, CompositeDisposable disposable)
        {
            _billboardPresenterFactory.CreateAndRun(arrowTransform, BillboardAxis.Y).AddTo(disposable);
        }
    }
}
