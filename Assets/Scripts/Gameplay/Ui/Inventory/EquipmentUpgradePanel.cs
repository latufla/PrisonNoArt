using Honeylab.Consumables;
using Honeylab.Gameplay.Analytics;
using Honeylab.Gameplay.Equipments;
using Honeylab.Utils.Configs;
using MPUIKIT;
using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;


namespace Honeylab.Gameplay.Ui
{
    public class EquipmentUpgradePanel : ScreenBase
    {
        [SerializeField] private GameObject _root;
        [SerializeField] private Button _backgroundButton;
        [SerializeField] private Image _equipmentIcon;
        [SerializeField] private TextMeshProUGUI _equipmentNameLabel;
        [SerializeField] private TextMeshProUGUI _equipmentDescriptionLabel;
        [SerializeField] private MPImage _upgradeLevelProgressImage;
        [SerializeField] private TextMeshProUGUI _upgradeLevelProgressLabel;
        [SerializeField] private UiConsumableItemInfo _attackInfo;
        [SerializeField] private UiConsumableItemInfo _healthInfo;
        [SerializeField] private UiConsumableItemInfo[] _uiConsumablesPriceInfo;
        [SerializeField] private Button _upgradeButton;
        [SerializeField] private Button _upgradeButtonDisabled;

        private EquipmentsData _equipmentsData;
        private EquipmentsService _equipmentsService;
        private IConfigsService _configsService;
        private ConsumablesService _consumablesService;
        private ConsumablesData _consumablesData;
        private CompositeDisposable _disposable;

        public Button BackgroundButton => _backgroundButton;
        public Button UpgradeButton => _upgradeButton;

        public override string Name => ScreenName.EquipmentUpgrade;
        public IObservable<Unit> OnUpgradeButtonClickAsObservable() => _upgradeButton.OnClickAsObservable();

        public override IObservable<Unit> OnCloseButtonClickAsObservable() => base.OnCloseButtonClickAsObservable()
            .Merge(_backgroundButton.OnClickAsObservable());


        public bool IsRunning { get; private set; }


        public void Init(EquipmentsData equipmentsData,
            EquipmentsService equipmentsService,
            IConfigsService configsService,
            ConsumablesService consumablesService,
            ConsumablesData consumablesData)
        {
            Hide();
            _equipmentsData = equipmentsData;
            _equipmentsService = equipmentsService;
            _configsService = configsService;
            _consumablesService = consumablesService;
            _consumablesData = consumablesData;
        }


        public void Run(EquipmentId id, ScreenOpenType screenOpenType, Action<string> onClickAction)
        {
            Clear();
            Show(screenOpenType);

            _disposable = new CompositeDisposable();

            IDisposable onCloseClick = OnCloseButtonClickAsObservable()
                .Subscribe(_ =>
                {
                    onClickAction.Invoke(ScreenParameters.EquipmentClose);
                    Clear();
                });
            _disposable.Add(onCloseClick);

            EquipmentData data = _equipmentsData.GetData(id);
            EquipmentConfig config = _configsService.Get<EquipmentConfig>(data.ConfigId);

            SetInfo(id, data, config);

            IDisposable onUpgradeClick = OnUpgradeButtonClickAsObservable()
                .Subscribe(_ =>
                {
                    onClickAction.Invoke(ScreenParameters.EquipmentUpgrade);

                    int nextLevel = _equipmentsService.GetEquipmentLevel(id).Value + 1;
                    if (_equipmentsService.HasEnoughConsumables(config, nextLevel))
                    {
                        if (_equipmentsService.TryEquipmentLevelUp(id))
                        {
                            EquipmentUpgradeLevelConfig upgradeConfig = _equipmentsService.GetUpgradeLevelConfig(config.Upgrade, nextLevel);
                            upgradeConfig.Price.ForEach(it => _consumablesService.ChangeAmount(it.Name, -it.Amount, new TransactionSource(TransactionName.EquipmentUpgrade, TransactionType.Equipments)));
                            SetInfo(id, data, config);
                        }
                    }
                });
            _disposable.Add(onUpgradeClick);

            IsRunning = true;
        }


        private void SetInfo(EquipmentId id, EquipmentData data, EquipmentConfig config)
        {
            int level = _equipmentsService.GetEquipmentLevel(id).Value;
            int nextLevel = level + 1;

            if (level < 0)
            {
                return;
            }

            bool hasEnoughAll = _equipmentsService.HasEnoughConsumables(config, nextLevel);
            _upgradeButton.gameObject.SetActive(hasEnoughAll);
            _upgradeButtonDisabled.gameObject.SetActive(!hasEnoughAll);

            _equipmentNameLabel.text = data.Title;
            _equipmentDescriptionLabel.text = data.Description;
            _equipmentIcon.sprite = data.Levels[level].Sprite;

            foreach (UiConsumableItemInfo itemInfo in _uiConsumablesPriceInfo)
            {
                itemInfo.gameObject.SetActive(false);
            }

            EquipmentUpgradeLevelConfig upgradeConfig = _equipmentsService.GetUpgradeLevelConfig(config.Upgrade, level);
            EquipmentUpgradeLevelConfig nextUpgradeConfig = _equipmentsService.GetUpgradeLevelConfig(config.Upgrade, nextLevel);

            SetUpgradeProgressInfo(nextLevel, config.Upgrade.GetLevels().Count, nextUpgradeConfig == null);

            if (upgradeConfig != null)
            {
                _attackInfo.SetAmount($"+{upgradeConfig.Damage}");
                _healthInfo.SetAmount($"+{upgradeConfig.Health}");

                _attackInfo.SetIcon(_equipmentsData.AttackPowerSprite);
                _healthInfo.SetIcon(_equipmentsData.HealthPointSprite);
            }

            if (nextUpgradeConfig == null)
            {
                return;
            }

            SetRequirementsInfo(nextUpgradeConfig);
        }


        private void SetUpgradeProgressInfo(int amount, int maxAmount, bool isMax)
        {
            _upgradeLevelProgressImage.fillAmount = !isMax ? (float)amount / maxAmount : 1;
            _upgradeLevelProgressLabel.text = !isMax ? $"{amount}/{maxAmount}" : "MAX";
        }


        private void SetRequirementsInfo(EquipmentUpgradeLevelConfig upgradeConfig)
        {
            for (int i = 0; i < upgradeConfig.Price.Count; i++)
            {
                var price = upgradeConfig.Price[i];
                ConsumableData consumableData = _consumablesData.GetData(price.Name);

                _uiConsumablesPriceInfo[i].gameObject.SetActive(true);
                _uiConsumablesPriceInfo[i].SetIcon(consumableData.Sprite);
                _uiConsumablesPriceInfo[i].SetAmount(price.Amount);

                bool hasEnough = _consumablesService.GetAmountProp(consumableData.Id).Value >= price.Amount;
                Color noMoneyColor = Color.red;
                if (!hasEnough)
                {
                    _uiConsumablesPriceInfo[i].SetColorText(noMoneyColor);
                }
                else
                {
                    _uiConsumablesPriceInfo[i].ResetColorText();
                }
            }
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
            foreach (UiConsumableItemInfo itemInfo in _uiConsumablesPriceInfo)
            {
                itemInfo.ResetColorText();
            }

            Hide();
            _disposable?.Dispose();
            _disposable = null;

            IsRunning = false;
        }
    }
}
