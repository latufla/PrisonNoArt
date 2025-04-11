using Zenject;


namespace Honeylab.Gameplay.Equipments
{
    public class EquipmentsInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<EquipmentsService>()
                .AsSingle();
        }
    }
}
