using Honeylab.Gameplay.Player;
using Honeylab.Gameplay.Upgrades;
using Honeylab.Gameplay.Weapons;
using Honeylab.Gameplay.Weapons.Upgrades;
using Honeylab.Gameplay.World;
using Honeylab.Utils.Configs;
using Honeylab.Utils.Pool;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;


namespace Honeylab.Gameplay.Ui.Upgrades
{
    public class WeaponUpgradeScreenResult : ScreenBase
    {
        [SerializeField] private Button _backgroundButton;
        [SerializeField] private Image _weaponIcon;
        [SerializeField] private TextMeshProUGUI _levelLabel;
        [SerializeField] private GameObjectPoolBehaviour _weaponInfoBreakableItemPool;
        [SerializeField] private Transform _weaponBreakableItemsRoot;
        [SerializeField] private TextMeshProUGUI _oldDamageText;
        [SerializeField] private TextMeshProUGUI _newDamageText;
        [SerializeField] private WorldObjectId _meleeWeaponTypeId;

        private List<UiWorldItemInfo> _items = new();
        private string _levelLabelFormat;
        private List<WorldObjectFlow> _targetFlows = new();
        private List<WeaponAttackTarget> _targets = new();

        private PlayerInputService _playerInputService;
        private IConfigsService _configsService;
        private WorldObjectsService _world;
        private IDisposable _blockInput;
        private WeaponsData _weaponsData;
        private PlayerInventoryService _playerInventoryService;

        public override string Name => ScreenName.WeaponUpgrade;
        public override IObservable<Unit> OnCloseButtonClickAsObservable() => base.OnCloseButtonClickAsObservable()
            .Merge(_backgroundButton.OnClickAsObservable());


        [Inject]
        public void Construct(PlayerInputService playerInputService,
            IConfigsService configsService,
            WorldObjectsService world,
            WeaponsData weaponsData,
            PlayerInventoryService playerInventoryService)
        {
            _playerInputService = playerInputService;
            _configsService = configsService;
            _world = world;
            _weaponsData = weaponsData;
            _playerInventoryService = playerInventoryService;
        }


        public void Run()
        {
            PlayerFlow player = _world.GetObjects<PlayerFlow>().FirstOrDefault();
            if (player == null)
            {
                return;
            }

            var weaponAgent = player.Get<WeaponAgent>();
            var weaponId = weaponAgent.GetWeaponByTypeFirstOrDefault(_meleeWeaponTypeId);
            var upgrade = _world.GetObject<UpgradeFlow>(weaponId);
            var weaponLevel = upgrade.GetLevel();

            WeaponData weaponData = _weaponsData.GetData(weaponId);
            WeaponLevelData weaponLevelData = weaponData.Levels[weaponLevel - 1];

            _blockInput = _playerInputService.BlockInput();

            _targetFlows = _world.GetRegisteredObjects()
                .Where(it => it.GetComponentInChildren<WeaponAttackTarget>(true) != null)
                .ToList();
            _targets = _targetFlows.Select(it => it.GetComponentInChildren<WeaponAttackTarget>(true))
                .ToList();

            SetLevelText(weaponLevel);
            SetIcon(weaponLevelData.Sprite);

            WeaponUpgradeConfig weaponConfig = _configsService.Get<WeaponUpgradeConfig>(upgrade.ConfigId);

            float oldDamage = 0.0f;
            var oldLevel = weaponLevel - 2;
            var currentLevel = weaponLevel - 1;

            if (oldLevel >= 0)
            {
                oldDamage = upgrade.GetLevelUpgradeConfig<WeaponUpgradeLevelConfig>(weaponConfig.Upgrade,
                        oldLevel)
                    .Damage;
            }

            float newDamage = upgrade.GetLevelUpgradeConfig<WeaponUpgradeLevelConfig>(weaponConfig.Upgrade,
                    currentLevel)
                .Damage;

            SetDamageText(oldDamage, newDamage);
            SetItems(weaponLevel);
        }


        private void SetItems(int level)
        {
            ClearItems();

            var items = _playerInventoryService.GetItemsByWeaponLevel(level);
            foreach (InventoryItem item in items)
            {
                GameObject go = _weaponInfoBreakableItemPool.Pop(true);
                if (go.TryGetComponent(out UiWorldItemInfo itemInfo))
                {
                    itemInfo.SetIcon(item.Data.Sprite);
                    itemInfo.transform.SetParent(_weaponBreakableItemsRoot, false);

                    _items.Add(itemInfo);
                }
                else
                {
                    _weaponInfoBreakableItemPool.Push(go);
                }
            }
        }


        private void SetLevelText(int level)
        {
            if (string.IsNullOrEmpty(_levelLabelFormat))
            {
                _levelLabelFormat = _levelLabel.text;
            }

            string amountText = level.ToString();
            _levelLabel.text = string.Format(_levelLabelFormat, amountText);
        }


        private void SetIcon(Sprite icon)
        {
            _weaponIcon.sprite = icon;
        }


        private void SetDamageText(float oldDamage, float newDamage)
        {
            _oldDamageText.text = oldDamage.ToString();
            _newDamageText.text = newDamage.ToString();
        }


        public void Stop()
        {
            ClearItems();

            _blockInput?.Dispose();
            _blockInput = null;
        }


        private void ClearItems()
        {
            foreach (UiWorldItemInfo item in _items)
            {
                _weaponInfoBreakableItemPool.Push(item.gameObject);
            }

            _items.Clear();
        }
    }
}
