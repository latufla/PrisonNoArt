using System.Collections.Generic;
using UnityEngine;
using Zenject;


namespace Honeylab.Gameplay.Analytics
{
    public class GameplayAnalyticsInstaller : MonoInstaller
    {
        [SerializeField] private TutorialAnalyticsArgs _tutorialAnalyticsArgs;
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<TutorialAnalyticsTracker>()
                .AsSingle()
                .WithArguments(_tutorialAnalyticsArgs);

            Container.Bind<IEnumerable<IAnalyticsTracker>>()
                .FromMethod(ctx => ctx.Container.ResolveAll<IAnalyticsTracker>())
                .AsSingle();
        }
    }
}
