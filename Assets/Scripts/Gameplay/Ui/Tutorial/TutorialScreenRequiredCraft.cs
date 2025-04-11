using Cysharp.Threading.Tasks;
using Honeylab.Consumables;
using Honeylab.Consumables.Configs;
using Honeylab.Gameplay.Buildings;
using Honeylab.Gameplay.Buildings.Config;
using Honeylab.Gameplay.Ui;
using Honeylab.Gameplay.World;
using Honeylab.Utils;
using Honeylab.Utils.Extensions;
using Honeylab.Utils.Pool;
using Honeylab.Utils.Tutorial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UniRx;
using UnityEngine;


namespace Honeylab.Gameplay.Tutorial
{
    public class TutorialScreenRequiredCraft : TutorialScreenRequiredItem
    {
        [SerializeField] private GameObject _consumablesGroup;
        [SerializeField] private TimeProgressPanel _timeProgressPanel;
        [SerializeField] private GameObject _completedPanel;
        [SerializeField] private GameObjectPoolBehaviour _pool;
        private List<ItemInfo> _items = new List<ItemInfo>();
        private WorldObjectsService _world;
        private CancellationTokenSource _cts;
        private CompositeDisposable _disposable;
        private ConsumablesData _consumablesData;
        private ConsumablesService _consumablesService;
        private TutorialScreenFocus _screenFocus;
        private bool _isCompleted;
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

            _timeProgressPanel.gameObject.SetActive(false);
            _completedPanel.SetActive(false);
            _consumablesGroup.SetActive(false);

            CraftBuildingFlow building = (CraftBuildingFlow)tutorialInfo.WorldObject;

            if (building.State.Value == CraftBuildingStates.Done)
            {
                ShowCompleted();
                return;
            }

            SetItems(building.Config);

            _cts = new CancellationTokenSource();
            CancellationToken ct = _cts.Token;
            WorkItems(building, tutorialInfo, ct).Forget();

            IDisposable updateProgress = building.TimeLeft.Subscribe(timeLeft =>
            {
                if (timeLeft > 0)
                {
                    if (building.TryGet(out GasBuildingView _))
                    {
                        int amount = building.GetCurrentAmount();
                        if (amount > 0)
                        {
                            ShowCompleted();
                            _disposable.Dispose();
                            return;
                        }

                        int amountLeft = building.CraftAmount.Value - amount;
                        _timeProgressPanel.SetTime(building.Config.CraftDuration -
                            (building.Config.CraftDuration * amountLeft - (float)timeLeft),
                            building.Config.CraftDuration);
                    }
                    else
                    {
                        _timeProgressPanel.SetTime((float)timeLeft,
                            building.Config.CraftDuration * building.CraftAmount.Value);
                    }

                    _timeProgressPanel.gameObject.SetActive(true);
                    _consumablesGroup.SetActive(false);
                }

                if (building.State.Value == CraftBuildingStates.Done)
                {
                    ShowCompleted();
                }
            });
            _disposable.Add(updateProgress);
        }


        private void SetItems(CraftBuildingConfig config)
        {
            ClearItems();
            foreach (RewardAmountConfig price in config.CraftPrice)
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


        private async UniTask WorkItems(CraftBuildingFlow building,
            TutorialInfo tutorialInfo,
            CancellationToken ct)
        {
            while (true)
            {
                await UniTask.Yield(ct);
                if (building.State.Value != CraftBuildingStates.Idle)
                {
                    continue;
                }

                _timeProgressPanel.gameObject.SetActive(false);
                _completedPanel.SetActive(false);
                _consumablesGroup.SetActive(true);

                foreach (ItemInfo item in _items)
                {
                    var amountProp = _consumablesService.GetAmountProp(item.Id);
                    ConsumableData data = _consumablesData.GetData(item.Id);

                    RewardAmountConfig config =
                        building.Config.CraftPrice.FirstOrDefault(it => it.Name.Equals(data.Name));

                    if (config == null)
                    {
                        throw new Exception("config not found");
                    }

                    int delta = config.Amount * (tutorialInfo.MaxAmountToCollect - tutorialInfo.CurrentAmountToCollect) - amountProp.Value;
                    if (delta <= 0)
                    {
                        item.Completed = true;
                        delta = 0;
                    }
                    else
                    {
                        item.Completed = false;
                    }

                    item.UIItem.SetAmount(delta);
                    item.UIItem.CheckMarkEnabled(item.Completed);
                    item.UIItem.AmountActive(!item.Completed);
                }
            }
        }


        private void ShowCompleted()
        {
            _consumablesGroup.SetActive(false);
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

            _disposable?.Dispose();
            _disposable = null;

            _cts?.CancelThenDispose();
            _cts = null;

            Root.SetActive(false);
            _consumablesGroup.SetActive(false);
            _completedPanel.SetActive(false);
            _timeProgressPanel.gameObject.SetActive(false);

            _isCompleted = false;
        }
    }
}
