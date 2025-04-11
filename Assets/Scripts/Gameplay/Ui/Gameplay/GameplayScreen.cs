using System.Collections.Generic;
using System.Linq;
using Honeylab.Consumables;
using Honeylab.Gameplay.Ui.AdOffer;
using Honeylab.Gameplay.Ui.Booster;
using Honeylab.Gameplay.Ui.Craft;
using Honeylab.Gameplay.Ui.Minimap;
using Honeylab.Gameplay.Ui.Pause;
using Honeylab.Gameplay.Ui.Upgrades;
using Honeylab.Utils.Pool;
using UnityEngine;

namespace Honeylab.Gameplay.Ui
{
    public class GameplayScreen : MonoBehaviour
    {
        [SerializeField] private Transform _consumableCountersRoot;
        [SerializeField] private GameObjectPoolBehaviour _consumableCountersPool;
        [SerializeField] private GameObjectPoolBehaviour _consumableCountersHardPool;
        [SerializeField] private CraftStatusButton _craftStatusButton;
        [SerializeField] private MinimapButton _minimapButton;
        [SerializeField] private AdOfferButton _adOfferButton;
        [SerializeField] private PauseButton _pauseButton;
        [SerializeField] private GameObject _hiderScreen;
        [SerializeField] private WeaponUpgradeStatusPanel _weaponUpgradeStatusPanel;
        [SerializeField] private PlayerInventoryButton _inventoryButton;
        [SerializeField] private WeaponBoosterButton _weaponBoosterButton;
        [SerializeField] private WeaponBoosterPanel _weaponBoosterPanel;

        private readonly List<CounterInfo> _counters = new();


        public Transform ConsumableCountersRoot => _consumableCountersRoot;
        public CraftStatusButton CraftStatusButton => _craftStatusButton;
        public MinimapButton MinimapButton => _minimapButton;
        public AdOfferButton AdOfferButton => _adOfferButton;
        public PauseButton PauseButton => _pauseButton;
        public GameObject HiderScreen => _hiderScreen;
        public WeaponUpgradeStatusPanel WeaponUpgradeStatusPanel => _weaponUpgradeStatusPanel;
        public PlayerInventoryButton InventoryButton => _inventoryButton;
        public WeaponBoosterButton WeaponBoosterButton => _weaponBoosterButton;
        public WeaponBoosterPanel WeaponBoosterPanel => _weaponBoosterPanel;


        private class CounterInfo
        {
            public ConsumablePersistenceId Id;
            public ConsumableCounterView CounterView;


            public CounterInfo(ConsumablePersistenceId id, ConsumableCounterView counter)
            {
                Id = id;
                CounterView = counter;
            }
        }


        public void Show()
        {
            gameObject.SetActive(true);
        }


        public void Hide()
        {
            gameObject.SetActive(false);
        }


        public ConsumableCounterView AddConsumableCounter(ConsumablePersistenceId id)
        {
            GameObject counterGo = _consumableCountersPool.Pop(true);
            counterGo.transform.SetParent(_consumableCountersRoot, false);
            ConsumableCounterView counter = counterGo.GetComponentInChildren<ConsumableCounterView>();
            _counters.Add(new CounterInfo(id, counter));
            return counter;
        }


        public ConsumableCounterHardView AddConsumableCounterHard(ConsumablePersistenceId id)
        {
            GameObject counterGo = _consumableCountersHardPool.Pop(true);
            counterGo.transform.SetParent(_consumableCountersRoot, false);
            ConsumableCounterHardView counter = counterGo.GetComponentInChildren<ConsumableCounterHardView>();
            _counters.Add(new CounterInfo(id, counter));
            return counter;
        }


        public ConsumableCounterView GetConsumableCounter(ConsumablePersistenceId id) =>
            _counters.FirstOrDefault(it => it.Id.Equals(id))?.CounterView;


        public void RemoveConsumableCounter(ConsumableCounterView view)
        {
            _consumableCountersPool.Push(view.gameObject);
            var counter = _counters.FirstOrDefault(it => it.CounterView.Equals(view));
            _counters.Remove(counter);
        }


        public void ConsumableCounterSetIndex(ConsumablePersistenceId id, int index)
        {
            var counterView = GetConsumableCounter(id);

            if (counterView.IsVisible())
            {
                return;
            }

            var counter = _counters.FirstOrDefault(it => it.CounterView.Equals(counterView));
            _counters.Remove(counter);
            _counters.Insert(index, counter);
            counterView.SetSiblingIndexIfNotVisible(index);
        }


        public void RefreshConsumablesCounter(ConsumablesService consumablesService, ConsumablesData consumablesData)
        {
            int index = 0;
            foreach (var counter in _counters)
            {
                ConsumablePersistenceId id = counter.Id;
                ConsumableData data = consumablesData.GetData(id);

                int amount = data.ConsumableType == ConsumableType.Hard ?
                    consumablesService.GetObservableAmount(data.Id).Value :
                    consumablesService.GetAmountProp(data.Id).Value;

                if ((amount > 0 || data.ConsumableType is ConsumableType.Soft or ConsumableType.Hard) &&
                    index < consumablesData.NumberVisibleConsumables)
                {
                    counter.CounterView.SetVisible(true);
                    index++;
                }
                else
                {
                    counter.CounterView.SetVisible(false);
                }
            }
        }


        public void Clear()
        {
            _weaponUpgradeStatusPanel.Clear();
        }
    }
}
