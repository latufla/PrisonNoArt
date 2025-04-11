using UnityEngine;
using Zenject;


namespace Honeylab.Gameplay.World
{
    public class WorldObjectsInstaller : MonoInstaller
    {
        [SerializeField] private WorldObjectsArgs _args;

        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<WorldObjectsService>()
                .AsSingle()
                .WithArguments(_args);
        }
    }
}
