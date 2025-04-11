using UnityEngine;
using Zenject;


namespace Honeylab.Utils
{
    public class VibrationInstaller : MonoInstaller
    {
        [SerializeField] private VibrationServiceParams _vibrationServiceParams;


        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<VibrationService>()
                .AsSingle()
                .WithArguments(_vibrationServiceParams);
        }
    }
}
