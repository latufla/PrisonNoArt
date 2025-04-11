using System;
using UniRx;
using UnityEngine;


namespace Honeylab.Gameplay.Ui
{
    public class BillboardPresenter : IDisposable
    {
        private readonly Transform _transform;
        private readonly Transform _gameCameraTransform;
        private BillboardAxis _axis;
        private IDisposable _disposable;


        public BillboardPresenter(Transform transform, Camera camera, BillboardAxis axis)
        {
            _transform = transform;
            _gameCameraTransform = camera.transform;

            SetAxis(axis);
        }


        public void Run()
        {
            RefreshRotation();
            _disposable = Observable.EveryUpdate()
                .Subscribe(_ => RefreshRotation());
        }


        public void Dispose()
        {
            _disposable?.Dispose();
            _disposable = null;
        }


        public void SetAxis(BillboardAxis axis)
        {
            _axis = axis;
        }


        private void RefreshRotation()
        {
            if (_transform == null)
                return;


            Vector3 previousEuler = _transform.rotation.eulerAngles;
            Vector3 cameraEuler = _gameCameraTransform.rotation.eulerAngles;

            Vector3 newEuler = previousEuler;
            if ((_axis & BillboardAxis.X) != 0)
            {
                newEuler.x = cameraEuler.x;
            }

            if ((_axis & BillboardAxis.Y) != 0)
            {
                newEuler.y = cameraEuler.y;
            }

            if ((_axis & BillboardAxis.Z) != 0)
            {
                newEuler.z = cameraEuler.z;
            }

            _transform.rotation = Quaternion.Euler(newEuler);
        }
    }
}
