using Zenject;


namespace Honeylab.Project.Cheats
{
    public class ProjectCheatsInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<SaveCheat>()
                .AsSingle()
                .NonLazy();
        }
    }
}
