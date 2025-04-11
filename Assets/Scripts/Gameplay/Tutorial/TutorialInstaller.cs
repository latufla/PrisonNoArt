using Honeylab.Gameplay.Tutorial;
using UnityEngine;
using Zenject;


namespace Honeylab.Utils.Tutorial
{
    public class TutorialInstaller : MonoInstaller
    {
        [SerializeField] private TutorialScreen _screen;


        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<TutorialScreenPresenter>()
                .AsSingle()
                .WithArguments(_screen);
        }
    }
}
