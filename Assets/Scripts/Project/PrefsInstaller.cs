using Honeylab.Utils.Prefs;
using Zenject;


namespace Honeylab.Project
{
    public class PrefsInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<UnityPrefsService>()
                .AsSingle();
        }
    }
}
