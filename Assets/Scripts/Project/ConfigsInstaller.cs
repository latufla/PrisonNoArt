using Honeylab.Utils.Configs;
using Zenject;


namespace Honeylab.Project
{
    public class ConfigsInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<RemoteConfigsService>()
                .AsSingle();
        }
    }
}
