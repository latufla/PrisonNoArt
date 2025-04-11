using Zenject;


namespace Honeylab.Gameplay.Player
{
    public class PlayerInventoryInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<PlayerInventoryService>()
                .AsSingle();
        }
    }
}
