using UnityEngine;
using Zenject;


namespace Honeylab.Sounds.Installers
{
    public class SoundInstaller : MonoInstaller
    {
        [SerializeField] private SoundServiceParams _soundServiceParams;


        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<SoundService>()
                .AsSingle()
                .WithArguments(_soundServiceParams);
        }
    }
}
