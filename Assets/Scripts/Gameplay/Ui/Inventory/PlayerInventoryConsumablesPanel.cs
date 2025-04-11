using Honeylab.Consumables;
using Honeylab.Sounds;
using Honeylab.Sounds.Data;
using Honeylab.Utils.Pool;
using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.UI;


namespace Honeylab.Gameplay.Ui
{
    [Serializable]
    public class InventoryPanel
    {
        [field: SerializeField]
        public ConsumableType ConsumableType { get; private set; }
        [field: SerializeField]
        public Transform Root { get; private set; }
        [field: SerializeField]
        public Button Button { get; private set; }
        [field: SerializeField]
        public ScrollRect ScrollRect { get; private set; }
    }
    public class PlayerInventoryConsumablesPanel : MonoBehaviour
    {
        [SerializeField] private GameObjectPoolBehaviour _consumableItemsPool;
        [SerializeField] private SoundId _enableSound;

        [SerializeField] private List<InventoryPanel> _inventoryPanels;

        private ConsumablesData _consumablesData;
        private ConsumablesService _consumablesService;
        private SoundService _soundService;
        private CompositeDisposable _disposable;
        private ConsumableType _currentType;

        private readonly List<UiConsumableItemInfo> _items = new();


        public void Init(ConsumablesService consumablesService,
            ConsumablesData consumablesData,
            SoundService soundService)
        {
            _consumablesData = consumablesData;
            _consumablesService = consumablesService;
            _soundService = soundService;
        }


        public void Run(Action<string> onClickAction)
        {
            _disposable = new CompositeDisposable();

            SetItems(ConsumableType.Regular);

            IDisposable onClickResources = _inventoryPanels.First(x => x.ConsumableType == ConsumableType.Regular).Button.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    onClickAction.Invoke(ScreenParameters.Resources);
                    SetItems(ConsumableType.Regular);
                });
            _disposable.Add(onClickResources);

            IDisposable onClickEquipmentsCard = _inventoryPanels.First(x => x.ConsumableType == ConsumableType.Card).Button.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    onClickAction.Invoke(ScreenParameters.Cards);
                    SetItems(ConsumableType.Card);
                });
            _disposable.Add(onClickEquipmentsCard);

            IDisposable onClickFinds = _inventoryPanels.First(x => x.ConsumableType == ConsumableType.Finds).Button.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    onClickAction.Invoke(ScreenParameters.Finds);
                    SetItems(ConsumableType.Finds);
                });
            _disposable.Add(onClickFinds);
        }


        private void SetItems(ConsumableType type)
        {
            ItemsClear();

            _currentType = type;

            if (_enableSound != null)
            {
                _soundService.RequestSoundPlay(_enableSound);
            }

            foreach (var panel in _inventoryPanels)
            {
                panel.Button.gameObject.SetActive(panel.ConsumableType != type);
                panel.ScrollRect.gameObject.SetActive(panel.ConsumableType == type);
            }

            var regularConsumables = _consumablesData.GetData(type);
            foreach (ConsumableData c in regularConsumables)
            {
                SetItem(c, type);
                // For Finds Set Unique View with button and click interaction
            }
        }


        private void SetItem(ConsumableData data, ConsumableType type)
        {
            Transform parent =_inventoryPanels.First(x => x.ConsumableType == type).Root.transform;
            var amountProp = _consumablesService.GetAmountProp(data.Id);
            if (amountProp.Value <= 0 && type != ConsumableType.Card)
            {
                return;
            }

            GameObject itemGo = _consumableItemsPool.Pop(true);
            if (itemGo.TryGetComponent(out UiConsumableItemInfo itemInfo))
            {
                Sprite icon = data.Sprite;
                itemInfo.SetIcon(icon);
                itemInfo.SetAmount(amountProp.Value);
                itemInfo.transform.SetParent(parent, false);
                _items.Add(itemInfo);
            }
            else
            {
                _consumableItemsPool.Push(itemGo);
            }
        }

        public void Refresh()
        {
            SetItems(_currentType);
        }


        private void ItemsClear()
        {
            foreach (UiConsumableItemInfo item in _items)
            {
                _consumableItemsPool.Push(item.gameObject);
            }

            _items.Clear();
        }


        public void Clear()
        {
            ItemsClear();

            _disposable?.Dispose();
            _disposable = null;
        }
    }
}
