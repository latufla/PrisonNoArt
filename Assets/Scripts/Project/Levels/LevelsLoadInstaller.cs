using UnityEngine;
using Zenject;


namespace Honeylab.Project.Levels
{
    public class LevelsLoadInstaller : MonoInstaller
    {
        [SerializeField] private LevelsServiceArgs _args;


        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<LevelsLoadService>()
                .AsSingle()
                .WithArguments(_args);
        }
    }
}
