using UnityEngine;
using Zenject;


namespace Honeylab.Gameplay.Ui.Minimap
{
    public class MinimapIndicatorsInstaller : MonoInstaller
    {
        [SerializeField] private MinimapIndicatorsServiceArgs _args;


        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<MinimapIndicatorsService>()
                .AsSingle()
                .WithArguments(_args);
        }
    }
}
