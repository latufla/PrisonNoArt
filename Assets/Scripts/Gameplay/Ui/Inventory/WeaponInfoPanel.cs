using Honeylab.Consumables;
using Honeylab.Gameplay.Player;
using Honeylab.Gameplay.Weapons;
using Honeylab.Utils.Pool;
using System;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;


namespace Honeylab.Gameplay.Ui.Weapons
{
    public class WeaponInfoPanel : ScreenBase
    {
        [SerializeField] private GameObject _root;

        [SerializeField] private Button _backgroundButton;
        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _levelLabel;
        [SerializeField] private Transform _weaponBreakableItemsRoot;
        [SerializeField] private GameObjectPoolBehaviour _weaponInfoBreakableItemPool;

        private CompositeDisposable _disposable;
        private string _levelLabelFormat;

        private List<UiWorldItemConsumablesInfo> _items = new();
        private PlayerInventoryService _playerInventoryService;
        private ConsumablesData _consumablesData;

        public override string Name => ScreenName.WeaponInfo;
        public override IObservable<Unit> OnCloseButtonClickAsObservable() => base.OnCloseButtonClickAsObservable()
            .Merge(_backgroundButton.OnClickAsObservable());


        public void Init(PlayerInventoryService playerInventoryService,
            ConsumablesData consumablesData)
        {
            Hide();
            _playerInventoryService = playerInventoryService;
            _consumablesData = consumablesData;
        }


        public void Run(WeaponLevelData weaponLevelData, int level, ScreenOpenType screenOpenType)
        {
            if (level < 0)
            {
                return;
            }

            _disposable?.Dispose();
            _disposable = new CompositeDisposable();

            IDisposable onWeaponClick = OnCloseButtonClickAsObservable()
                .Subscribe(_ => { Clear(); });
            _disposable.Add(onWeaponClick);

            Show(screenOpenType);

            SetIcon(weaponLevelData.Sprite);
            SetText(level);
            SetItems(level + 1);
        }


        private void SetItems(int level)
        {
            ClearItems();

            var items = _playerInventoryService.GetItemsByWeaponLevel(level, true);
            foreach (InventoryItem item in items)
            {
                var rewardNames = item.Target.GetRewardNames();
                if (rewardNames == null)
                {
                    continue;
                }
                GameObject go = _weaponInfoBreakableItemPool.Pop(true);
                if (go.TryGetComponent(out UiWorldItemConsumablesInfo itemInfo))
                {
                    itemInfo.SetIcon(item.Data.Sprite);
                    itemInfo.transform.SetParent(_weaponBreakableItemsRoot, false);
                    List<Sprite> sprites = new List<Sprite>();
                    foreach (string rewardName in rewardNames)
                    {
                        Sprite sprite = _consumablesData.GetData(rewardName).Sprite;
                        sprites.Add(sprite);
                    }
                    itemInfo.AddConsumableIcons(sprites);

                    _items.Add(itemInfo);
                }
                else
                {
                    _weaponInfoBreakableItemPool.Push(go);
                }
            }
        }


        private void SetText(int level)
        {
            if (string.IsNullOrEmpty(_levelLabelFormat))
            {
                _levelLabelFormat = _levelLabel.text;
            }

            string amountText = (level + 1).ToString();
            _levelLabel.text = string.Format(_levelLabelFormat, amountText);
        }


        private void SetIcon(Sprite icon)
        {
            _icon.sprite = icon;
        }


        public override void Hide()
        {
            _root.SetActive(false);
        }


        public override void Show(ScreenOpenType screenOpenType)
        {
            _screenOpenType = screenOpenType;
            _root.SetActive(true);
        }


        public void Clear()
        {
            ClearItems();
            Hide();

            _disposable?.Dispose();
            _disposable = null;
        }


        private void ClearItems()
        {
            foreach (UiWorldItemConsumablesInfo item in _items)
            {
                _weaponInfoBreakableItemPool.Push(item.gameObject);
            }

            _items.Clear();
        }

        internal void Run(WeaponLevelData weaponLevelData, int level, ScreenOpenType screenOpenType1, object screenOpenType2)
        {
            throw new NotImplementedException();
        }
    }
}
