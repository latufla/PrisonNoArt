using Unity.AI.Navigation;
using UnityEngine;
using Zenject;


namespace Honeylab.Gameplay.Creatures
{
    public class NavigationInstaller : MonoInstaller
    {
        [SerializeField] private NavMeshSurface[] _surfaces;


        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<NavigationService>()
                .FromMethod(ctx => new NavigationService(_surfaces))
                .AsSingle();
        }
    }
}
