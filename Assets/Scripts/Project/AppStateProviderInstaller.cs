using Honeylab.Utils.App;
using Zenject;


namespace Honeylab.Project
{
    public class AppStateProviderInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<AppStateProviderBehaviour>()
                .FromNewComponentOnNewGameObject()
                .WithGameObjectName(nameof(AppStateProviderBehaviour))
                .AsSingle()
                .OnInstantiated((InjectContext _, AppStateProviderBehaviour behaviour) =>
                    DontDestroyOnLoad(behaviour.gameObject));
        }
    }
}
