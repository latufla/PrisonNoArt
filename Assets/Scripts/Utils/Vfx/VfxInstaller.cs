using Zenject;


namespace Honeylab.Utils.Vfx
{
    public class VfxInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<VfxService>()
                .AsSingle();
        }
    }
}
