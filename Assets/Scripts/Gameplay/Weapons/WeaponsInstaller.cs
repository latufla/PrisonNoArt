using Zenject;


namespace Honeylab.Gameplay.Weapons
{
    public class WeaponsInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<WeaponsFactory>()
                .AsSingle();
        }
    }
}
