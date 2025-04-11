using Honeylab.Consumables;
using Honeylab.Gameplay.Equipments;
using Honeylab.Utils.Configs;
using System;
using System.Linq;
using UniRx;
using UnityEngine;


namespace Honeylab.Gameplay.Ui
{
    public class PlayerInventoryPanel : MonoBehaviour
    {
        [SerializeField] private GameObject _modelView;
        [SerializeField] private EquipmentUpgradePanel _equipmentUpgradePanel;
        [SerializeField] private Sprite _equipmentSlotDefaultSprite;
        [SerializeField] private InventoryEquipmentSlot[] _equipmentSlots;
        [SerializeField] private UiConsumableItemInfo _combatPowerInfo;
        [SerializeField] private UiConsumableItemInfo _attackInfo;
        [SerializeField] private UiConsumableItemInfo _healthInfo;

        private EquipmentsService _equipmentsService;
        private EquipmentsData _equipmentsData;
        private ConsumablesData _consumablesData;
        private ConsumablesService _consumablesService;
        private IConfigsService _configsService;
        private Subject<EquipmentId> _onEquipmentUpgradeAvailable = new Subject<EquipmentId>();

        private CompositeDisposable _disposable;

        public EquipmentUpgradePanel EquipmentUpgradePanel => _equipmentUpgradePanel;
        public IObservable<EquipmentId> OnEquipmentUpgradeAvailableAsObservable() => _onEquipmentUpgradeAvailable.AsObservable();


        public void Init(EquipmentsData equipmentsData,
            EquipmentsService equipmentsService,
            IConfigsService configsService,
            ConsumablesService consumablesService,
            ConsumablesData consumablesData)
        {
            _equipmentsService = equipmentsService;
            _equipmentsData = equipmentsData;
            _configsService = configsService;
            _consumablesService = consumablesService;
            _consumablesData = consumablesData;

            _equipmentUpgradePanel.Init(_equipmentsData,
                _equipmentsService,
                _configsService,
                _consumablesService,
                _consumablesData);
        }


        public void Run(Action<string> onClickAction)
        {
            _disposable?.Dispose();
            _disposable = new CompositeDisposable();

            _modelView.SetActive(true);

            RunEquipments(onClickAction);
        }


        private void RunEquipments(Action<string> onClickAction)
        {
            SetInfoStats();

            foreach (InventoryEquipmentSlot slot in _equipmentSlots)
            {
                int level = _equipmentsService.HasEquipment(slot.Id) ?
                    _equipmentsService.GetEquipmentLevel(slot.Id).Value :
                    -1;

                slot.ItemInfo.NotificationInit();
                slot.ItemInfo.SetActiveIconPanel(level >= 0);

                if (level < 0)
                {
                    continue;
                }

                Sprite sprite = _equipmentsData.GetData(slot.Id).Levels[level].Sprite;
                slot.ItemInfo.SetIcon(sprite);
                slot.ItemInfo.SetAmount(level + 1);

                _equipmentsService.GetEquipmentLevel(slot.Id)
                    .Subscribe(newLevel =>
                    {
                        Sprite newSprite = _equipmentsData.GetData(slot.Id).Levels[newLevel].Sprite;
                        slot.ItemInfo.SetIcon(newSprite);
                        slot.ItemInfo.SetAmount(newLevel + 1);
                    })
                    .AddTo(_disposable);

                slot.ItemInfo.OnButtonClickAsObservable()
                    .Subscribe(_ =>
                    {
                        onClickAction.Invoke(ScreenParameters.Equipment);
                        _equipmentUpgradePanel.Run(slot.Id, ScreenOpenType.RequiredClick, onClickAction);
                    })
                    .AddTo(_disposable);
            }
        }


        private void SetInfoStats()
        {
            _combatPowerInfo.SetAmount(_equipmentsService.CombatPower.Value);
            _combatPowerInfo.SetIcon(_equipmentsData.CombatPowerSprite);
            _attackInfo.SetAmount(_equipmentsService.AttackPower);
            _attackInfo.SetIcon(_equipmentsData.AttackPowerSprite);
            _healthInfo.SetAmount(_equipmentsService.HealthPower);
            _healthInfo.SetIcon(_equipmentsData.HealthPointSprite);

            IDisposable onEquipmentsUpdate = _equipmentsService.CombatPower
                .Subscribe(combatPower =>
                {
                    _combatPowerInfo.SetAmount(combatPower);
                    _attackInfo.SetAmount(_equipmentsService.AttackPower);
                    _healthInfo.SetAmount(_equipmentsService.HealthPower);
                });
            _disposable.Add(onEquipmentsUpdate);
        }


        public void Clear()
        {
            _modelView.SetActive(false);

            _equipmentUpgradePanel.Clear();

            _disposable?.Dispose();
            _disposable = null;
        }


        public void EquipmentsNotificationActive(bool isActive, EquipmentId id)
        {
            InventoryEquipmentSlot slot = _equipmentSlots
                .FirstOrDefault(it => it.Id != null && it.Id.Equals(id));

            if (slot == null)
            {
                return;
            }

            if (slot.ItemInfo.NotificationIsActive != isActive && isActive)
            {
                _onEquipmentUpgradeAvailable.OnNext(id);
            }

            slot.ItemInfo.NotificationActive(isActive);
        }

        public UIEquipmentsSlotInfo GetSlotItem(EquipmentId id) => _equipmentSlots.First(it => it.Id.Equals(id)).ItemInfo;


        [Serializable]
        private class InventoryEquipmentSlot
        {
            [SerializeField] private EquipmentId _id;
            [SerializeField] private UIEquipmentsSlotInfo _itemInfo;


            public EquipmentId Id => _id;
            public UIEquipmentsSlotInfo ItemInfo => _itemInfo;
        }
    }
}
