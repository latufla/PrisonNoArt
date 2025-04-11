using Cysharp.Threading.Tasks;
using Honeylab.Consumables;
using Honeylab.Consumables.Configs;
using Honeylab.Gameplay.Buildings;
using Honeylab.Gameplay.Buildings.Config;
using Honeylab.Gameplay.SpeedUp;
using Honeylab.Gameplay.Ui;
using Honeylab.Gameplay.World;
using Honeylab.Utils;
using Honeylab.Utils.Extensions;
using Honeylab.Utils.Pool;
using Honeylab.Utils.Tutorial;
using System;
using System.Collections.Generic;
using System.Threading;
using UniRx;
using UnityEngine;


namespace Honeylab.Gameplay.Tutorial
{
    public class TutorialScreenRequiredConsumables : TutorialScreenRequiredItem
    {
        [SerializeField] private GameObject _consumablesGroup;
        [SerializeField] private TimeProgressPanel _timeProgressPanel;
        [SerializeField] private GameObject _completedPanel;
        [SerializeField] private GameObjectPoolBehaviour _pool;
        private List<ItemInfo> _items = new List<ItemInfo>();

        private WorldObjectsService _world;
        private TutorialScreenFocus _screenFocus;
        private ConsumablesData _consumablesData;
        private ConsumablesService _consumablesService;

        private CancellationTokenSource _cts;
        private CompositeDisposable _disposable;
        private bool _isCompleted = false;
        public override bool IsCompleted() => _isCompleted;


        public override void Init(TutorialFlow flow, TutorialScreenFocus screenFocus)
        {
            _world = flow.Resolve<WorldObjectsService>();
            _consumablesData = flow.Resolve<ConsumablesData>();
            _consumablesService = flow.Resolve<ConsumablesService>();
            _screenFocus = screenFocus;
        }


        public override void Run(TutorialInfo tutorialInfo)
        {
            if (tutorialInfo.WorldObject == null)
            {
                return;
            }

            _disposable?.Dispose();
            _disposable = new CompositeDisposable();

            Root.SetActive(true);

            ShowRequiredConsumables(tutorialInfo);
        }


        private void ShowRequiredConsumables(TutorialInfo tutorialInfo)
        {
            UnlockBuildingFlow building = tutorialInfo.WorldObject as UnlockBuildingFlow;
            if (building == null)
            {
                throw new Exception("unlockBuilding is not found");
            }

            SetItems(building.Config);

            _cts = new CancellationTokenSource();
            CancellationToken ct = _cts.Token;

            ItemsWork(building, ct).Forget();

            IDisposable updateProgress = building.TimeLeft.Subscribe(timeLeft =>
            {
                _timeProgressPanel.SetTime((float)timeLeft, building.Duration);
            });
            _disposable.Add(updateProgress);
        }


        private void SetItems(UnlockBuildingConfig config)
        {
            ClearItems();
            foreach (RewardAmountConfig price in config.UnlockPrice)
            {
                ConsumableData data = _consumablesData.GetData(price.Name);

                GameObject itemGO = _pool.Pop(true);
                if (itemGO.TryGetComponent(out UIItemInfoWithCheckMark item))
                {
                    itemGO.transform.SetParent(_consumablesGroup.transform);
                    itemGO.transform.localScale = Vector3.one;
                    _items.Add(new ItemInfo() { UIItem = item, Id = data.Id, Completed = false });

                    item.AmountActive(true);
                    item.CheckMarkEnabled(false);
                    item.SetIcon(data.Sprite);
                }
                else
                {
                    _pool.Push(itemGO);
                }
            }
        }


        private async UniTask ItemsWork(UnlockBuildingFlow building, CancellationToken ct)
        {
            _timeProgressPanel.gameObject.SetActive(false);
            _completedPanel.SetActive(false);
            _consumablesGroup.SetActive(true);

            while (true)
            {
                await UniTask.Yield(ct);
                int i = _items.Count;
                foreach (var item in _items)
                {
                    if (item.Completed)
                    {
                        i--;
                        continue;
                    }

                    var amountProp = _consumablesService.GetAmountProp(item.Id);
                    ConsumableData data = _consumablesData.GetData(item.Id);

                    int index = building.Price.FindIndex(it => it.Name.Equals(data.Name));
                    RewardAmountConfig config = building.Price[index];

                    var currentAmount = building.Consumables.Amounts[index];
                    var delta = config.Amount - amountProp.Value - currentAmount;
                    if (delta <= 0 || currentAmount >= config.Amount)
                    {
                        item.Completed = true;
                        item.UIItem.AmountActive(false);
                        item.UIItem.CheckMarkEnabled(true);
                    }
                    else
                    {
                        item.UIItem.SetAmount(delta);
                    }
                }

                if (i <= 0)
                {
                    break;
                }
            }

            if (building.State.Value == UnlockBuildingStates.Idle)
            {
                await UniTask.WaitUntil(() => building.State.Value == UnlockBuildingStates.Progress,
                    cancellationToken: ct);
            }

            _consumablesGroup.SetActive(false);
            Transform focusTarget = building.transform;
            if (focusTarget != null)
            {
                _screenFocus.SetTarget(focusTarget);
            }

            _timeProgressPanel.gameObject.SetActive(true);

            await UniTask.WaitUntil(() => building.State.Value != UnlockBuildingStates.Progress, cancellationToken: ct);
            ShowCompleted();
        }


        private void ShowCompleted()
        {
            _timeProgressPanel.gameObject.SetActive(false);
            _completedPanel.SetActive(true);
            _isCompleted = true;
        }


        private void ClearItems()
        {
            foreach (ItemInfo item in _items)
            {
                _pool.Push(item.UIItem.gameObject);
            }
            _items.Clear();
        }


        public override void Hide()
        {
            ClearItems();

            _cts?.CancelThenDispose();
            _cts = null;

            _disposable?.Dispose();
            _disposable = null;

            Root.SetActive(false);
            _consumablesGroup.SetActive(false);
            _timeProgressPanel.gameObject.SetActive(false);
            _completedPanel.SetActive(false);

            _isCompleted = false;
        }
    }
}
