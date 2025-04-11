using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Honeylab.Consumables;
using Honeylab.Gameplay.Player;
using Honeylab.Gameplay.Ui;
using Honeylab.Gameplay.World;
using Honeylab.Persistence;
using Honeylab.Utils.Arrows;
using Honeylab.Utils.CameraTargeting;
using Honeylab.Utils.OffscreenTargetIndicators;
using Honeylab.Utils.Persistence;
using Honeylab.Utils.Tutorial;
using UniRx;
using UnityEngine;
using Zenject;

namespace Honeylab.Gameplay.Tutorial
{
    public abstract class TutorialStepBase : MonoBehaviour
    {
        [SerializeField] private WorldObjectIdProvider _id;

        private LevelPersistenceService _levelPersistenceService;
        private OffscreenIndicatorsService _offscreenIndicatorsService;
        private ArrowsPool _arrowsPool;
        private UiArrowsPool _uiArrowsPool;
        private ConsumablesData _consumablesData;
        private ConsumablesService _consumablesService;
        private WorldObjectsService _world;
        private TutorialScreenPresenter _screenPresenter;
        private PlayerInputService _playerInputService;
        protected ICameraTargetingService CameraTargetingService;
        private ToastsService _toastsService;

        private readonly ConcurrentQueue<Func<UniTask>> _queue = new();
        private TutorialPersistentComponent _state;
        private TutorialFlow _tutorialFlow;
        private OffscreenIndicator _offscreenIndicator;


        public WorldObjectId Id => _id != null ? _id.Id : null;

        public abstract TutorialInfo GetTutorialInfo();

        protected PlayerInputService PlayerInputService => _playerInputService;
        protected OffscreenIndicatorsService OffscreenIndicatorsService => _offscreenIndicatorsService;
        protected ConsumablesData ConsumablesData => _consumablesData;


        [Inject]
        public void Construct(LevelPersistenceService levelPersistenceService,
            TutorialScreenPresenter screenPresenter,
            ICameraTargetingService cameraTargetingService,
            OffscreenIndicatorsService offscreenIndicatorsService,
            ArrowsPool arrowsPool,
            UiArrowsPool uiArrowsPool,
            PlayerInputService playerInputService,
            WorldObjectsService world,
            ConsumablesData consumablesData,
            ConsumablesService consumablesService,
            ToastsService toastsService)
        {
            _levelPersistenceService = levelPersistenceService;
            _screenPresenter = screenPresenter;
            CameraTargetingService = cameraTargetingService;
            _offscreenIndicatorsService = offscreenIndicatorsService;
            _arrowsPool = arrowsPool;
            _playerInputService = playerInputService;
            _world = world;
            _consumablesData = consumablesData;
            _consumablesService = consumablesService;
            _toastsService = toastsService;
            _uiArrowsPool = uiArrowsPool;
        }


        public void Init()
        {
            _state = Id ? _levelPersistenceService.GetComponent<TutorialPersistentComponent>(_id.Id) : null;
            _tutorialFlow = _world.GetObjects<TutorialFlow>().First();
        }


        public async UniTask RunAsync(CancellationToken ct)
        {
            if (_state == null)
            {
                return;
            }

            await WaitRunAsync(ct);

            var subSteps = CreateSubSteps(ct);
            subSteps.ForEach(it => _queue.Enqueue(it));

            int progress = 0;
            while (!_queue.IsEmpty)
            {
                _queue.TryDequeue(out var subStep);

                progress++;
                if (_state.Progress + 1 > progress)
                {
                    continue;
                }

                await subStep();

                _state.Progress++;

                OnSubStepComplete();

                await _screenPresenter.RunCompleteAsync();

                await UniTask.WaitUntil(() => !PlayerInputService.IsBlocked, cancellationToken: ct);
            }
        }


        protected abstract void OnSubStepComplete();


        protected virtual async UniTask WaitRunAsync(CancellationToken ct)
        {
            await GetObjectFirstAsync<PlayerFlow>(ct);
        }


        protected abstract List<Func<UniTask>> CreateSubSteps(CancellationToken ct);


        protected void SendStepStartAnalytics(TutorialInfo info)
        {
            _tutorialFlow.OnStepStart(new TutorialStepInfo
            {
                MissionId = info.TaskText
            });
        }


        protected void SendStepStartAnalytics(string info)
        {
            _tutorialFlow.OnStepStart(new TutorialStepInfo
            {
                MissionId = info
            });
        }


        protected void SendStepEndAnalytics()
        {
            _tutorialFlow.OnStepEnd();
        }


        protected void FocusTargetAsync(Transform target) => _screenPresenter.FocusTarget(target);


        protected UniTask FocusTargetAsync(Transform target, float duration, CancellationToken ct) =>
            _screenPresenter.FocusTargetAsync(target, duration, ct);


        protected IArrowHandle ShowTargetArrow(Transform target, float offsetY)
        {
            Vector3 arrowPosition = target.position;
            arrowPosition.y += offsetY;
            IArrowHandle arrow = _arrowsPool.PopAndShowArrow(arrowPosition);
            return arrow;
        }


        protected IArrowHandle ShowTargetUiArrow(Transform target, float offsetY, bool activeSortingLayer = false)
        {
            Vector3 arrowPosition = target.position;
            arrowPosition.y += offsetY;
            IArrowHandle arrow = _uiArrowsPool.PopAndShowArrow(arrowPosition);

            if (activeSortingLayer)
            {
                ArrowHandle handle = (ArrowHandle)arrow;
                UiArrowView view = (UiArrowView)handle.Arrow;
                view.ActiveSortingLayer();
            }

            return arrow;
        }


        protected void HideTargetArrow(IArrowHandle arrow)
        {
            _arrowsPool.HideAsync(arrow).Forget();
        }


        protected OffscreenIndicator ShowOffscreenIndicator(Transform target, Sprite icon = null)
        {
            _offscreenIndicator = _offscreenIndicatorsService.Add(_tutorialFlow.OffscreenIndicatorId, target);
            if (icon != null)
            {
                _offscreenIndicator.SetIcon(icon);
            }

            return _offscreenIndicator;
        }


        protected void HideOffscreenIndicator()
        {
            if (_offscreenIndicator != null)
            {
                _offscreenIndicatorsService.Remove(_offscreenIndicator);
                _offscreenIndicator = null;
            }
        }


        protected void ShowScreen(TutorialInfo info, Transform target = null, bool activeSortingLayer = false)
        {
            _screenPresenter.Hide();
            _screenPresenter.Show(info, target, activeSortingLayer);
        }


        protected void HideScreen()
        {
            _screenPresenter.Hide();
        }


        protected void ScreenSetCollectedAmount(int amount)
        {
            _screenPresenter.SetCollectedAmount(amount);
        }


        protected IReadOnlyList<T> GetObjects<T>() where T : WorldObjectFlow => _world.GetObjects<T>();
        protected T GetObject<T>(WorldObjectId id) where T : WorldObjectFlow => _world.GetObject<T>(id);
        protected WorldObjectFlow GetObject(WorldObjectId id) => _world.GetObject(id);


        protected async UniTask<T> GetObjectAsync<T>(WorldObjectId id, CancellationToken ct) where T : WorldObjectFlow
        {
            T result = null;
            await UniTask.WaitUntil(() =>
                {
                    result = GetObject<T>(id);
                    return result != null;
                },
                cancellationToken: ct);

            return result;
        }


        protected async UniTask<T> GetObjectFirstAsync<T>(CancellationToken ct) where T : WorldObjectFlow
        {
            T result = null;
            await UniTask.WaitUntil(() =>
                {
                    result = GetObjects<T>().FirstOrDefault();
                    return result != null;
                },
                cancellationToken: ct);

            return result;
        }


        protected async UniTask<WorldObjectFlow> GetObjectAsync(WorldObjectId id, CancellationToken ct) =>
            await GetObjectAsync<WorldObjectFlow>(id, ct);


        protected ConsumableData GetConsumableData(ConsumablePersistenceId id) => _consumablesData.GetData(id);
        protected ConsumableData GetConsumableData(string consumableName) => _consumablesData.GetData(consumableName);


        protected IReadOnlyReactiveProperty<int> GetConsumableAmountProp(ConsumablePersistenceId id) =>
            _consumablesService.GetAmountProp(id);


        protected void SetFocusTarget(Transform target)
        {
            _screenPresenter.SetFocusTarget(target);
        }


        protected IDisposable SetHighlightInfoIcon(WorldObjectFlow targetFlow, Sprite sprite, CancellationToken ct)
        {
            return _screenPresenter.ScreenFocus.OnStartFocusSubjectsAsObservable()
                .Subscribe(async _ =>
                {
                    var health = targetFlow.Get<Health>();

                    if (health != null && health.HealthProp.Value >= 0)
                        await HighlightInfoIcon(targetFlow, sprite, ct);
                });
        }


        public async UniTask HighlightInfoIcon(WorldObjectFlow targetFlow, Sprite sprite, CancellationToken ct)
        {
            var resourceHighlightItem = targetFlow.Get<ResourceHighlightItem>();
            if (resourceHighlightItem != null)
            {
                await resourceHighlightItem.ShowItem(sprite, ct);
            }
            else
            {
                Debug.LogError($"{targetFlow.name} ResourceHighlightItem not found");
            }
        }


        protected void ShowToast(Vector3 position, string text, float time)
        {
            _toastsService.ShowSpeechToast(position, text, time);
        }


        protected IDisposable BlockPlayerInput() => PlayerInputService.BlockInput();


        protected ICameraTargetingHandle ChangeCameraTarget(Transform target) => CameraTargetingService.Enqueue(target);


        protected T Resolve<T>() => _tutorialFlow.Resolve<T>();


        public virtual void Clear()
        {
        }
    }
}