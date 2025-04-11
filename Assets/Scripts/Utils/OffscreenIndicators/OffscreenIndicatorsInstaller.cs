using UnityEngine;
using Zenject;


namespace Honeylab.Utils.OffscreenTargetIndicators
{

    public class OffscreenIndicatorsInstaller : MonoInstaller
    {
        [SerializeField] private OffscreenIndicatorsServiceArgs _args;

        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<OffscreenIndicatorsService>()
                .AsSingle()
                .WithArguments(_args);
        }
    }
}
