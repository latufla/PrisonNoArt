using UnityEngine;
using Zenject;


namespace Honeylab.Gameplay.Startup
{
    public class GameplayStartupInstaller : MonoInstaller
    {
        [SerializeField] private GameObjectContext _levelPrefab;


        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<GameplayStartup>()
                .AsSingle()
                .WithArguments(_levelPrefab);
        }
    }
}
