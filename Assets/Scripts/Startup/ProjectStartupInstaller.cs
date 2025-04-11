using UnityEngine;
using Zenject;


namespace Honeylab.Startup
{
    public class ProjectStartupInstaller : MonoInstaller
    {
        [SerializeField] private LoadingScreen _loadingScreen;


        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<ProjectStartup>()
                .AsSingle();

            Container.BindInstance(_loadingScreen)
                .AsSingle();
        }
    }
}
