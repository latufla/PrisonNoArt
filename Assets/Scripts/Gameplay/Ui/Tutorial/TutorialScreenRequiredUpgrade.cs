using Cysharp.Threading.Tasks;
using Honeylab.Consumables;
using Honeylab.Consumables.Configs;
using Honeylab.Gameplay.Buildings;
using Honeylab.Gameplay.Ui;
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
    public class TutorialScreenRequiredUpgrade : TutorialScreenRequiredItem
    {
        [SerializeField] private GameObject _consumablesGroup;
        [SerializeField] private TimeProgressPanel _timeProgressPanel;
        [SerializeField] private GameObject _completedPanel;
        [SerializeField] private GameObjectPoolBehaviour _pool;
        private List<ItemInfo> _items = new List<ItemInfo>();
        private CancellationTokenSource _cts;
        private CompositeDisposable _disposable;
        private ConsumablesData _consumablesData;
        private ConsumablesService _consumablesService;
        private bool _isCompleted = false;
        public override bool IsCompleted() => _isCompleted;


        public override void Init(TutorialFlow flow, TutorialScreenFocus screenFocus)
        {
            _consumablesData = flow.Resolve<ConsumablesData>();
            _consumablesService = flow.Resolve<ConsumablesService>();
        }


        public override void Run(TutorialInfo tutorialInfo)
        {
            if (tutorialInfo.WorldObject == null)
            {
                return;
            }

            UpgradeBuildingFlow building = (UpgradeBuildingFlow)tutorialInfo.WorldObject;

            if (building == null || building.WeaponUpgradeConfig == null)
            {
                return;
            }

            _disposable?.Dispose();
            _disposable = new CompositeDisposable();

            Root.SetActive(true);

            _cts = new CancellationTokenSource();
            CancellationToken ct = _cts.Token;
            WorkItems(building, tutorialInfo, ct).Forget();

            IDisposable upgradeProgress = building.TimeLeft.Subscribe(timeLeft =>
            {
                if (timeLeft <= 0)
                {
                    return;
                }

                _timeProgressPanel.SetTime((float)timeLeft, building.WeaponUpgradeConfig.Duration);
                _timeProgressPanel.gameObject.SetActive(true);
                _consumablesGroup.SetActive(false);
            });
            _disposable.Add(upgradeProgress);

            IDisposable onStateChange = building.State.ValueProperty.Subscribe(state =>
            {
                switch (state)
                {
                    case UpgradeBuildingStates.Done:
                        ShowCompleted();
                        break;
                    case UpgradeBuildingStates.Idle:
                    {
                        int curLevel = building.WeaponUpgrade.UpgradeLevelPersistence.Value + 1;
                        if (curLevel < tutorialInfo.RequiredWeaponLevel)
                        {
                            SetItems(building.WeaponUpgradeConfig.Price);
                        }
                        else
                        {
                            ShowCompleted();
                        }

                        break;
                    }
                }
            });
            _disposable.Add(onStateChange);
        }


        private void SetItems(List<RewardAmountConfig> allPrice)
        {
            _timeProgressPanel.gameObject.SetActive(false);
            _completedPanel.SetActive(false);
            _consumablesGroup.SetActive(false);

            ClearItems();

            foreach (RewardAmountConfig price in allPrice)
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


        private async UniTask WorkItems(UpgradeBuildingFlow building,
            TutorialInfo tutorialInfo,
            CancellationToken ct)
        {
            while (true)
            {
                await UniTask.Yield(ct);
                if (building.State.Value != UpgradeBuildingStates.Idle)
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
                        building.WeaponUpgradeConfig.Price.FirstOrDefault(it => it.Name.Equals(data.Name));

                    if (config == null)
                    {
                        throw new Exception("config not found");
                    }

                    int delta = config.Amount - amountProp.Value;
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
