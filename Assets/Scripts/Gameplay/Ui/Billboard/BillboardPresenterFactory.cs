using System;
using UnityEngine;


namespace Honeylab.Gameplay.Ui
{
    public class BillboardPresenterFactory
    {
        private readonly Camera _camera;


        public BillboardPresenterFactory(Camera camera)
        {
            _camera = camera;
        }


        public IDisposable CreateAndRun(Transform transform, BillboardAxis axis = BillboardAxis.All)
        {
            BillboardPresenter presenter = Create(transform, axis);
            presenter.Run();
            return presenter;
        }


        public BillboardPresenter Create(Transform transform, BillboardAxis axis = BillboardAxis.All) =>
            new BillboardPresenter(transform, _camera, axis);
    }
}
