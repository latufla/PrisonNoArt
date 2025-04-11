using Honeylab.Consumables;
using Honeylab.Gameplay.Equipments;
using Honeylab.Gameplay.Player;
using Honeylab.Gameplay.Ui.Weapons;
using Honeylab.Gameplay.Weapons;
using Honeylab.Sounds;
using Honeylab.Utils.Configs;
using Honeylab.Utils.OffscreenTargetIndicators;
using System;
using UniRx;
using UnityEngine;


namespace Honeylab.Gameplay.Ui
{
    public class PlayerInventoryScreen : ScreenBase
    {
        [SerializeField] private Canvas _canvas;

        [SerializeField] private ConsumablePersistenceId _moneyId;
        [SerializeField] private UiConsumableItemInfo _consumableMoneyInfo;
        [SerializeField] private PlayerInventoryPanel _playerInventoryPanel;
        [SerializeField] private PlayerInventoryConsumablesPanel _consumablesPanel;
        [SerializeField] private WeaponInfoPanel _weaponInfoPanel;
        [SerializeField] private UiWeaponItemPanel _uiWeaponItemPanel;


        private WeaponsData _weaponsData;
        private PlayerInputService _playerInputService;
        private EquipmentsService _equipmentsService;
        private EquipmentsData _equipmentsData;
        private ConsumablesData _consumablesData;
        private ConsumablesService _consumablesService;
        private IConfigsService _configsService;
        private OffscreenIndicatorsService _offscreenIndicatorsService;

        private CompositeDisposable _disposable;

        public PlayerInventoryPanel PlayerInventoryPanel => _playerInventoryPanel;

        public override string Name => ScreenName.PlayerInventory;


        public void Init(PlayerInputService playerInputService,
            WeaponsData weaponsData,
            ConsumablesData consumablesData,
            ConsumablesService consumablesService,
            PlayerInventoryService playerInventoryService,
            EquipmentsService equipmentsService,
            EquipmentsData equipmentsData,
            IConfigsService configsService,
            SoundService soundService,
            OffscreenIndicatorsService offscreenIndicatorsService)
        {
            _weaponsData = weaponsData;
            _playerInputService = playerInputService;
            _equipmentsService = equipmentsService;
            _equipmentsData = equipmentsData;
            _consumablesData = consumablesData;
            _consumablesService = consumablesService;
            _configsService = configsService;
            _offscreenIndicatorsService = offscreenIndicatorsService;

            _consumablesPanel.Init(_consumablesService, _consumablesData, soundService);
            _weaponInfoPanel.Init(playerInventoryService, _consumablesData);
            _playerInventoryPanel.Init(_equipmentsData,
                _equipmentsService,
                _configsService,
                _consumablesService,
                _consumablesData);
        }


        public void Run(WeaponInfo weaponInfo)
        {
            _disposable?.Dispose();
            _disposable = new CompositeDisposable();

            IDisposable blockInput = _playerInputService.BlockInput();
            _disposable.Add(blockInput);

            _offscreenIndicatorsService.SetIndicatorsVisible(false);

            Sprite moneySpite = _consumablesData.GetData(_moneyId)?.Sprite;
            int amount = _consumablesService.GetAmountProp(_moneyId).Value;
            _consumableMoneyInfo.SetIcon(moneySpite);
            _consumableMoneyInfo.SetAmount(amount);

            IDisposable onMoneyUpdate = _consumablesService.GetAmountProp(_moneyId)
                .Subscribe(am => _consumableMoneyInfo.SetAmount(am));
            _disposable.Add(onMoneyUpdate);

            WeaponPanelConfig(weaponInfo, ScreenAction);

            _playerInventoryPanel.Run(ScreenAction);
            _consumablesPanel.Run(ScreenAction);

            IDisposable onChangeAmount = _consumablesService
                .OnConsumablesChangeAmountAsObservable()
                .Subscribe(_ => _consumablesPanel.Refresh());
            _disposable.Add(onChangeAmount);
        }


        private void WeaponPanelConfig(WeaponInfo weaponInfo, Action<string> onClickAction)
        {
            var upgradePersistence = weaponInfo.Upgrade.UpgradeLevelPersistence;
            IDisposable onWeaponClick = _uiWeaponItemPanel.OnWeaponButtonClickAsObservable()
                .Subscribe(_ =>
                {
                    onClickAction.Invoke(ScreenParameters.Weapon);

                    int level = upgradePersistence.Value;
                    WeaponData weaponData = _weaponsData.GetData(weaponInfo.WeaponId);
                    WeaponLevelData weaponLevelData = weaponData.Levels[level];

                    _weaponInfoPanel.Run(weaponLevelData, level, ScreenOpenType.RequiredClick);
                });
            _disposable.Add(onWeaponClick);

            IDisposable onWeaponCloseClick = _weaponInfoPanel.OnCloseButtonClickAsObservable()
                .Subscribe(_ => onClickAction.Invoke(ScreenParameters.WeaponClose));
            _disposable.Add(onWeaponCloseClick);


            int level = upgradePersistence.Value;
            WeaponData weaponData = _weaponsData.GetData(weaponInfo.WeaponId);
            WeaponLevelData weaponLevelData = weaponData.Levels[level];

            _uiWeaponItemPanel.SetIcon(weaponLevelData.Sprite);
            _uiWeaponItemPanel.SetLevel(level + 1);
        }


        public override void Show(ScreenOpenType screenOpenType)
        {
            base.Show(screenOpenType);
            _canvas.overrideSorting = true;
        }


        public override void Hide()
        {
            base.Hide();
            _canvas.overrideSorting = false;
            Clear();
        }


        private void Clear()
        {
            _disposable?.Dispose();
            _disposable = null;

            _weaponInfoPanel.Clear();

            _playerInventoryPanel.Clear();
            _consumablesPanel.Clear();

            _offscreenIndicatorsService?.SetIndicatorsVisible(true);
        }
    }
}
