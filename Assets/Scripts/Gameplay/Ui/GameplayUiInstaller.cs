using Honeylab.Gameplay.Equipments;
using Honeylab.Gameplay.Pools;
using Honeylab.Gameplay.Ui.AdOffer;
using Honeylab.Gameplay.Ui.CombatPower;
using Honeylab.Gameplay.Ui.Craft;
using Honeylab.Gameplay.Ui.Minimap;
using Honeylab.Gameplay.Ui.Pause;
using Honeylab.Gameplay.Ui.Speedup;
using Honeylab.Gameplay.Ui.Upgrades;
using Honeylab.Utils;
using Honeylab.Utils.Arrows;
using Honeylab.Utils.Pool;
using UnityEngine;
using Zenject;


namespace Honeylab.Gameplay.Ui
{
    public class GameplayUiInstaller : MonoInstaller
    {
        [SerializeField] private GameplayScreen _gameplayScreen;
        [SerializeField] private Transform _toastsRoot;
        [SerializeField] private Transform _arrowsRoot;
        [SerializeField] private Transform _uiArrowsRoot;
        [SerializeField] private RectTransform _uiScreenParent;

        public override void InstallBindings()
        {
            Container.BindInstance(_gameplayScreen);
            Container.BindInterfacesAndSelfTo<GameplayScreenPresenter>()
                .AsSingle();

            Container.BindInterfacesAndSelfTo<ToastsService>()
                .AsSingle()
                .WithArguments(_toastsRoot);

            Container.BindInterfacesAndSelfTo<ArrowsPool>()
                .AsSingle()
                .WithArguments(_arrowsRoot);

            Container.BindInterfacesAndSelfTo<UiArrowsPool>()
                .AsSingle()
                .WithArguments(_uiArrowsRoot);

            Container.BindInterfacesAndSelfTo<PointingArrow>()
                .AsSingle();

            Container.Bind<IGameObjectPool>()
                .FromMethod(ctx =>
                {
                    GameplayPoolsService pools = ctx.Container.Resolve<GameplayPoolsService>();
                    return pools.Get<Honeylab.Pools.ArrowsPool>();
                })
                .AsCached()
                .WhenInjectedInto<ArrowsPool>();

            Container.Bind<IGameObjectPool>()
                .FromMethod(ctx =>
                {
                    GameplayPoolsService pools = ctx.Container.Resolve<GameplayPoolsService>();
                    return pools.Get<Honeylab.Pools.UiArrowsPool>();
                })
                .AsCached()
                .WhenInjectedInto<UiArrowsPool>();

            Container.BindInterfacesAndSelfTo<ArrowsService>()
                .AsSingle();

            Container.BindInterfacesAndSelfTo<ScreenFactory>()
                .AsSingle()
                .WithArguments(_uiScreenParent);
        }
    }
}
