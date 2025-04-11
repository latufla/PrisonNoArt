using UnityEngine;
using Zenject;


namespace Honeylab.Cutscene
{
    public class CutsceneInstaller : MonoInstaller
    {
        [SerializeField] private CutsceneArgs _args;

        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<CutsceneService>()
                .AsSingle()
                .WithArguments(_args);
        }
    }
}
