using Honeylab.Consumables;
using Zenject;


namespace Honeylab.Project
{
    public class ConsumablesInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<ConsumablesService>()
                .AsSingle();
        }
    }
}
