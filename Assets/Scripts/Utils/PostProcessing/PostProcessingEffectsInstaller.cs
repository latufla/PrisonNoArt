using UnityEngine;
using Zenject;


namespace Honeylab.Utils
{
    public class PostProcessingEffectsInstaller : MonoInstaller
    {
        [SerializeField] private PostProcessingEffectsServiceArgs _args;


        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<PostProcessingEffectsService>()
                .AsSingle()
                .WithArguments(_args);
        }
    }
}
