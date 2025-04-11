using Cinemachine;
using Honeylab.Gameplay.Ui;
using Honeylab.Utils.CameraTargeting;
using UnityEngine;
using Zenject;


namespace Honeylab.Gameplay.Cameras
{
    public class CameraInstaller : MonoInstaller
    {
        [SerializeField] private CameraProvider _provider;
        [SerializeField] private CinemachineBrain _cinemachineBrain;


        public override void InstallBindings()
        {
            Container.BindInstance(_provider);
            Container.BindInstance(_cinemachineBrain);

            Container.BindInterfacesAndSelfTo<CameraTargetingService>()
                .AsSingle()
                .WithArguments(_provider.PlayerCamera);

            Container.BindInterfacesAndSelfTo<BillboardPresenterFactory>()
                .AsSingle()
                .WithArguments(_provider.GameCamera);
        }
    }
}
