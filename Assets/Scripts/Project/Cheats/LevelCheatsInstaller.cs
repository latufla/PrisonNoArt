using Zenject;


namespace Honeylab.Project.Cheats
{
    public class LevelCheatsInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<LevelsCheat>()
                .AsSingle()
                .NonLazy();

            Container.BindInterfacesTo<PlayerCheat>()
                .AsSingle()
                .NonLazy();

            Container.BindInterfacesTo<ConsumablesCheat>()
                .AsSingle()
                .NonLazy();

            Container.BindInterfacesTo<EquipmentsCheat>()
                .AsSingle()
                .NonLazy();
        }
    }
}
