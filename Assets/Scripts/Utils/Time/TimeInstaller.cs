using Zenject;


namespace Honeylab.Utils
{
    public class TimeInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<TimeService>()
                .AsSingle();
        }
    }
}
