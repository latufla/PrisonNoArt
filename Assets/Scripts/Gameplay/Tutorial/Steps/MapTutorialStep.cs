using Cysharp.Threading.Tasks;
using Honeylab.Consumables;
using Honeylab.Gameplay.Ui;
using Honeylab.Gameplay.Ui.Minimap;
using Honeylab.Gameplay.World;
using Honeylab.Persistence;
using Honeylab.Utils.Arrows;
using Honeylab.Utils.Persistence;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Zenject;


namespace Honeylab.Gameplay.Tutorial
{
    public class MapTutorialStep : TutorialStepBase
    {
        [SerializeField] private WorldObjectId _worldObjectId;
        [SerializeField] private ConsumablePersistenceId _pickupableId;
        [SerializeField] private TutorialInfo _info;
        [SerializeField] private TutorialInfo _mapInfo;
        [SerializeField] private string _mapText;
        private GameplayScreenPresenter _gameplayScreenPresenter;
        private ConsumablesService _consumablesService;
        private UiArrowsPool _uiArrowsPool;
        private GameplayScreen _gameplayScreen;

        [Inject]
        public void Construct(ConsumablesService consumablesService,
            GameplayScreenPresenter gameplayScreenPresenter,
            UiArrowsPool uiArrowsPool,
            GameplayScreen gameplayScreen)
        {
            _consumablesService = consumablesService;
            _gameplayScreenPresenter = gameplayScreenPresenter;
            _uiArrowsPool = uiArrowsPool;
            _gameplayScreen = gameplayScreen;
        }

        protected override List<Func<UniTask>> CreateSubSteps(CancellationToken ct) => new()
        {
            async () => await PickupMapAsync(ct),
            async () => await MapAsync(ct)
        };

        private async UniTask PickupMapAsync(CancellationToken ct)
        {
            var curAmountProp = _consumablesService.GetAmountProp(_pickupableId);
            SendStepStartAnalytics(_info);
            if (curAmountProp.Value > 0)
            {
                return;
            }

            Transform target = null;

            if (_worldObjectId != null)
            {
                WorldObjectFlow wo = await GetObjectAsync(_worldObjectId, ct);
                target = wo.transform;
            }

            ShowScreen(_info, target);

            if (_worldObjectId != null)
            {
                if (Resolve<LevelPersistenceService>().TryGet(_worldObjectId, out PersistentObject po))
                {
                    if (po.TryGetFirst<ReactiveValuePersistentComponent<bool>>(out var isDeactive))
                    {
                        if (isDeactive is { Value: true })
                        {
                            return;
                        }
                    }
                }
            }

            IArrowHandle arrow = null;
            if (target != null)
            {
                arrow = ShowTargetArrow(target, _info.ArrowPositionY);
                ShowOffscreenIndicator(target, _info.Icon);

                if (_info.FocusOnStart)
                {
                    FocusTargetAsync(target);
                }
            }

            if (curAmountProp.Value > 0)
            {
                return;
            }

            await UniTask.WaitUntil(() => curAmountProp.Value > 0, cancellationToken: ct);

            if (arrow != null)
            {
                HideTargetArrow(arrow);
            }
            HideOffscreenIndicator();
        }


        private async UniTask MapAsync(CancellationToken ct)
        {
            ShowScreen(_mapInfo, activeSortingLayer: true);

            Transform targetUi = _gameplayScreen.MinimapButton.transform;
            float uiArrowPosY = (float)_info.ArrowPositionY / 20;
            IArrowHandle arrowUi = ShowTargetUiArrow(targetUi, uiArrowPosY, activeSortingLayer: true);


            var hiderScreen = _gameplayScreen.HiderScreen;
            hiderScreen.SetActive(true);
            var oldParent = targetUi.parent;
            targetUi.SetParent(hiderScreen.transform);

            MinimapScreenPresenter minimapScreenPresenter = null;

            await UniTask.WaitUntil(() =>
                {
                    minimapScreenPresenter = _gameplayScreenPresenter.MinimapScreenPresenter;
                    return minimapScreenPresenter != null;
                },
                cancellationToken: ct);

            await UniTask.WaitUntil(() => minimapScreenPresenter.IsRunning, cancellationToken: ct);

            targetUi.SetParent(oldParent);
            hiderScreen.SetActive(false);

            _uiArrowsPool.HideAsync(arrowUi).Forget();

            MinimapScreen minimapScreen = minimapScreenPresenter.Screen;

            minimapScreen.TutorialSetText(_mapText);
            minimapScreen.PlayHandAnimation(true);

            List<UniTask> asyncList = new()
            {
                minimapScreen.OnDragAsObservable().ToUniTask(true, cancellationToken: ct),
                minimapScreen.MinimapZoomSlider.OnZoomSliderChangeAsObserver().ToUniTask(true, cancellationToken: ct),
                UniTask.WaitUntil(() => !minimapScreenPresenter.IsRunning, cancellationToken: ct)
            };

            await UniTask.WhenAny(asyncList);

            minimapScreen.PlayHandAnimation(false);

            await UniTask.WaitUntil(() => !minimapScreenPresenter.IsRunning, cancellationToken: ct);

            minimapScreen.TutorialSetText(string.Empty);
        }


        public override TutorialInfo GetTutorialInfo() => _info;


        protected override void OnSubStepComplete()
        {
            SendStepEndAnalytics();
        }
    }
}
