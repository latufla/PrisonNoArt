using UnityEngine;
using Zenject;


namespace Honeylab.Gameplay.Pools
{
    public class GameplayPoolsInstaller : MonoInstaller
    {
        [SerializeField] private GameplayPoolsService _poolsService;


        public override void InstallBindings()
        {
            Container.Bind<GameplayPoolsService>()
                .FromInstance(_poolsService)
                .AsSingle();
        }
    }
}
