using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Honeylab.Ads;
using Honeylab.Consumables;
using Honeylab.Consumables.Configs;
using Honeylab.Gameplay.Booster;
using Honeylab.Gameplay.Buildings;
using Honeylab.Gameplay.Cameras;
using Honeylab.Gameplay.Equipments;
using Honeylab.Gameplay.Player;
using Honeylab.Gameplay.Ui.AdOffer;
using Honeylab.Gameplay.Ui.Booster;
using Honeylab.Gameplay.Ui.CombatPower;
using Honeylab.Gameplay.Ui.Craft;
using Honeylab.Gameplay.Ui.Inventory;
using Honeylab.Gameplay.Ui.Minimap;
using Honeylab.Gameplay.Ui.Pause;
using Honeylab.Gameplay.Ui.Upgrades;
using Honeylab.Gameplay.Weapons;
using Honeylab.Gameplay.World;
using Honeylab.Persistence;
using Honeylab.Project.Levels;
using Honeylab.Sounds;
using Honeylab.Utils;
using Honeylab.Utils.CameraTargeting;
using Honeylab.Utils.Configs;
using Honeylab.Utils.Extensions;
using Honeylab.Utils.OffscreenTargetIndicators;
using Honeylab.Utils.Persistence;
using UniRx;
using UnityEngine;

namespace Honeylab.Gameplay.Ui
{
    public class GameplayScreenPresenter : IDisposable
    {
        private readonly CompositeDisposable _disposable = new();

        private readonly GameplayScreen _screen;
        private readonly ConsumablesService _consumablesService;
        private readonly ConsumablesData _consumablesData;

        private readonly List<ConsumableData> _consumables = new();

        private readonly PlayerInputService _playerInputService;
        private readonly CameraProvider _cameraProvider;
        private readonly WorldObjectsService _world;
        private readonly TimeService _timeService;
        private readonly OffscreenIndicatorsService _offscreenIndicatorsService;
        private readonly MinimapIndicatorsService _minimapIndicatorsService;
        private readonly ToastsService _toastsService;
        private readonly SoundService _soundService;
        private readonly VibrationService _vibrationService;
        private readonly EquipmentsService _equipmentsService;
        private readonly EquipmentsData _equipmentsData;
        private readonly RewardedAdsService _rewardedAdsService;
        private readonly PlayerInventoryService _playerInventoryService;
        private readonly ICameraTargetingService _cameraTargetingService;
        private readonly IConfigsService _configsService;
        private readonly LevelsLoadService _levelsLoadService;
        private readonly ScreenFactory _screenFactory;
        private readonly BoosterService _boosterService;
        private readonly LevelPersistenceService _levelPersistenceService;

        private CraftStatusScreenPresenter _craftStatusScreenPresenter;
        private CancellationTokenSource _craftCts;

        private MinimapScreenPresenter _minimapScreenPresenter;
        private CancellationTokenSource _minimapCts;

        private CombatPowerRequirementScreenPresenter _combatPowerRequirementScreenPresenter;
        private CancellationTokenSource _combatPowerCts;

        private AdOfferScreenPresenter _adOfferScreenPresenter;
        private CancellationTokenSource _adOfferCts;

        private PauseScreenPresenter _pauseScreenPresenter;
        private CancellationTokenSource _pauseScreenCts;

        private PlayerInventoryPresenter _inventoryPresenter;
        private CancellationTokenSource _inventoryScreenCts;

        private WeaponUpgradeScreenPresenter _weaponUpgradeScreenPresenter;
        private CancellationTokenSource _weaponUpgradeScreenCts;

        private CancellationTokenSource _simpleGetCts;

        private readonly List<ConsumableCounterView> _counterViews = new();
        private readonly List<ConsumableCounterHardView> _counterHardViews = new();

        private CancellationTokenSource _cts = new();

        public CraftStatusScreenPresenter CraftStatusScreenPresenter => _craftStatusScreenPresenter;
        public MinimapScreenPresenter MinimapScreenPresenter => _minimapScreenPresenter;

        public CombatPowerRequirementScreenPresenter CombatPowerRequirementScreenPresenter =>
            _combatPowerRequirementScreenPresenter;

        public AdOfferScreenPresenter AdOfferScreenPresenter => _adOfferScreenPresenter;
        public PauseScreenPresenter PauseScreenPresenter => _pauseScreenPresenter;
        public PlayerInventoryPresenter PlayerInventoryPresenter => _inventoryPresenter;

        private readonly List<ConsumablePersistenceId> _flyingConsumables = new List<ConsumablePersistenceId>();


        public GameplayScreenPresenter(GameplayScreen screen,
            ConsumablesService consumablesService,
            ConsumablesData consumablesData,
            PlayerInputService playerInputService,
            CameraProvider cameraProvider,
            WorldObjectsService world,
            TimeService timeService,
            OffscreenIndicatorsService offscreenIndicatorsService,
            MinimapIndicatorsService minimapIndicatorsService,
            ToastsService toastsService,
            EquipmentsService equipmentsService,
            EquipmentsData equipmentsData,
            RewardedAdsService rewardedAdsService,
            IConfigsService configsService,
            LevelsLoadService levelsLoadService,
            SoundService soundService,
            VibrationService vibrationService,
            ScreenFactory screenFactory,
            PlayerInventoryService playerInventoryService,
            ICameraTargetingService cameraTargetingService,
            BoosterService boosterService,
            LevelPersistenceService levelPersistenceService)
        {
            _screen = screen;
            _consumablesService = consumablesService;
            _consumablesData = consumablesData;
            _playerInputService = playerInputService;
            _cameraProvider = cameraProvider;
            _world = world;
            _timeService = timeService;
            _offscreenIndicatorsService = offscreenIndicatorsService;
            _minimapIndicatorsService = minimapIndicatorsService;
            _toastsService = toastsService;
            _equipmentsService = equipmentsService;
            _equipmentsData = equipmentsData;
            _rewardedAdsService = rewardedAdsService;
            _configsService = configsService;
            _levelsLoadService = levelsLoadService;
            _soundService = soundService;
            _vibrationService = vibrationService;
            _screenFactory = screenFactory;
            _playerInventoryService = playerInventoryService;
            _cameraTargetingService = cameraTargetingService;
            _boosterService = boosterService;
            _levelPersistenceService = levelPersistenceService;
        }


        public void Run()
        {
            _consumables.AddRange(_consumablesData.GetData(ConsumableType.Hard));
            _consumables.AddRange(_consumablesData.GetData(ConsumableType.Soft));
            _consumables.AddRange(_consumablesData.GetData(ConsumableType.Regular));

            _consumables.ForEach(RunConsumableCounter);
            _screen.RefreshConsumablesCounter(_consumablesService, _consumablesData);

            CancellationToken ct = _cts.Token;
            RunCraftStatus();
            RunWeaponAsync(ct).Forget();
            RunMinimap();
            RunAdOffer();
            RunCombatPowerRequirementScreen();
            RunPause();
            RunWeaponDamageBoost(ct).Forget();
        }


        public void Dispose()
        {
            _screen.Clear();

            _counterViews.ForEach(it => _screen.RemoveConsumableCounter(it));
            _counterViews.Clear();

            _counterHardViews.ForEach(it => _screen.RemoveConsumableCounter(it));
            _counterHardViews.Clear();

            _cts?.CancelThenDispose();
            _cts = null;

            _craftCts?.CancelThenDispose();
            _craftCts = null;

            _minimapCts?.CancelThenDispose();
            _minimapCts = null;

            _combatPowerCts?.CancelThenDispose();
            _combatPowerCts = null;

            _adOfferCts?.CancelThenDispose();
            _adOfferCts = null;

            _pauseScreenCts?.CancelThenDispose();
            _pauseScreenCts = null;

            _inventoryScreenCts?.CancelThenDispose();
            _inventoryScreenCts = null;
            _inventoryPresenter.Clear();

            _simpleGetCts?.CancelThenDispose();
            _simpleGetCts = null;

            _weaponUpgradeScreenCts?.CancelThenDispose();
            _weaponUpgradeScreenCts = null;

            _disposable.Dispose();
        }


        private void RunConsumableCounter(ConsumableData data)
        {
            if (data.ConsumableType is ConsumableType.Hard)
            {
                ConsumableCounterHardView counterHard = _screen.AddConsumableCounterHard(data.Id);
                RunHardConsumableCounter(data, counterHard);
                counterHard.SetAsFirstSibling();
            }
            else if (data.ConsumableType is ConsumableType.Soft)
            {
                ConsumableCounterHardView counterHard = _screen.AddConsumableCounterHard(data.Id);
                RunSoftConsumableCounter(data, counterHard);
                counterHard.SetAsFirstSibling();
            }
            else
            {
                ConsumableCounterView counter = _screen.AddConsumableCounter(data.Id);
                RunRegularConsumableCounter(data, counter);
            }
        }


        private void RunHardConsumableCounter(ConsumableData data, ConsumableCounterHardView counterView)
        {
            ObservableValuePersistentComponent observableAmount = _consumablesService.GetObservableAmount(data.Id);
            counterView.SetAmount(observableAmount.Value);
            counterView.SetIcon(data.Sprite);

            _counterHardViews.Add(counterView);

            IDisposable amountChange = observableAmount.OnValueChangeAsObservable()
                .Subscribe(amount =>
                {
                    if (!_flyingConsumables.Contains(data.Id))
                    {
                        counterView.SetAmount(amount, true);
                    }
                });
            _disposable.Add(amountChange);
        }


        private void RunSoftConsumableCounter(ConsumableData data, ConsumableCounterHardView counterView)
        {
            var amountProp = _consumablesService.GetAmountProp(data.Id);
            counterView.SetAmount(amountProp.Value);
            counterView.SetIcon(data.Sprite);
            _counterViews.Add(counterView);

            IDisposable amountChange = amountProp.Skip(1)
                .Subscribe(amount =>
                {
                    if (!_flyingConsumables.Contains(data.Id))
                    {
                        counterView.SetAmount(amount, true);
                    }
                });
            _disposable.Add(amountChange);
        }


        private void RunRegularConsumableCounter(ConsumableData data, ConsumableCounterView counterView)
        {
            var amountProp = _consumablesService.GetAmountProp(data.Id);
            counterView.SetAmount(amountProp.Value);
            counterView.SetIcon(data.Sprite);

            _counterViews.Add(counterView);

            int siblingIndex = _consumables.Count(it => it.ConsumableType is ConsumableType.Soft) +
                               _consumables.Count(it => it.ConsumableType is ConsumableType.Hard);

            IDisposable amountChange = amountProp.Skip(1)
                .Subscribe(amount =>
                {
                    if (!_flyingConsumables.Contains(data.Id))
                    {
                        counterView.SetAmount(amount, true);
                    }

                    _screen.ConsumableCounterSetIndex(data.Id, siblingIndex);
                    _screen.RefreshConsumablesCounter(_consumablesService, _consumablesData);
                });
            _disposable.Add(amountChange);
        }


        private async UniTask RunWeaponAsync(CancellationToken ct)
        {
            WeaponsData weaponsData = _levelsLoadService.GetActiveLevelData().WeaponsData;

            WeaponUpgradeStatusPanel weaponStatusPanel = _screen.WeaponUpgradeStatusPanel;
            weaponStatusPanel.Hide();

            _inventoryPresenter = new PlayerInventoryPresenter(_screenFactory,
                _playerInputService,
                weaponsData,
                _consumablesData,
                _consumablesService,
                _offscreenIndicatorsService,
                _playerInventoryService,
                _equipmentsService,
                _equipmentsData,
                _configsService,
                _soundService);

            _weaponUpgradeScreenPresenter = new WeaponUpgradeScreenPresenter(_screenFactory);


            WorldObjectId weaponTypeId =
                weaponsData.GetInventoryWeaponTypeId() ?? throw new Exception("WeaponId not found");
            WeaponInfo weaponInfo = null;

            _screen.InventoryButton.gameObject.SetActive(false);

            await UniTask.WaitUntil(() =>
                {
                    weaponInfo = GetInventoryWeaponInfo();
                    return weaponInfo != null;
                },
                cancellationToken: ct);

            _inventoryPresenter.Init(weaponTypeId);

            IDisposable inventoryClick = _screen.InventoryButton.OnButtonClickAsObservable()
                .Subscribe(_ => { RunInventoryScreen(ScreenOpenType.RequiredClick); });
            _disposable.Add(inventoryClick);

            IDisposable equipmentAvailable = _inventoryPresenter.OnAnyEquipmentUpgradeAvailableAsObservable()
                .Subscribe(_screen.InventoryButton.NotificationActive);
            _disposable.Add(equipmentAvailable);

            IDisposable weaponRun = weaponInfo.Upgrade.UpgradeLevelPersistence.ValueProperty
                .Skip(1)
                .Subscribe(_ =>
                {
                    _weaponUpgradeScreenCts?.CancelThenDispose();
                    _weaponUpgradeScreenCts = new CancellationTokenSource();
                    _weaponUpgradeScreenPresenter.RunAsync(ScreenOpenType.Auto, _weaponUpgradeScreenCts.Token).Forget();
                });
            _disposable.Add(weaponRun);

            weaponStatusPanel.Init(_playerInputService, _cameraTargetingService, weaponsData);

            IDisposable weaponUpgrade = weaponInfo.Upgrade.UpgradeLevelPersistence.ValueProperty
                .Subscribe(level =>
                {
                    WeaponData weaponData = weaponsData.GetData(weaponInfo.WeaponId);
                    WeaponLevelData weaponLevelData = weaponData.Levels[level];
                    weaponStatusPanel.SetInfo(level + 1, weaponLevelData.Sprite);
                });
            _disposable.Add(weaponUpgrade);

            EquipmentsData equipmentsData = _levelsLoadService.GetActiveLevelData().EquipmentsData;
            EquipmentId equipmentId = equipmentsData.GetInventoryEquipmentId() ??
                                      throw new Exception("EquipmentId not found");
            var inventoryProp = _equipmentsService.GetEquipmentLevel(equipmentId);
            IDisposable onInventoryHave = inventoryProp
                .Subscribe(level => { _screen.InventoryButton.gameObject.SetActive(level >= 0); });
            _disposable.Add(onInventoryHave);
        }


        private WeaponInfo GetInventoryWeaponInfo()
        {
            WeaponsData weaponsData = _levelsLoadService.GetActiveLevelData().WeaponsData;

            WorldObjectId weaponTypeId =
                weaponsData.GetInventoryWeaponTypeId() ?? throw new Exception("WeaponId not found");
            WeaponInfo weaponInfo = _playerInventoryService.GetWeaponUpgradeInfo(weaponTypeId);
            return weaponInfo;
        }


        private void RunInventoryScreen(ScreenOpenType screenOpenType)
        {
            _inventoryScreenCts?.CancelThenDispose();
            _inventoryScreenCts = new CancellationTokenSource();

            _inventoryPresenter.RunAsync(screenOpenType, _inventoryScreenCts.Token).Forget();
        }


        private async UniTask RunWeaponDamageBoost(CancellationToken ct)
        {
            WeaponUpgradeStatusPanel weaponUpgradeStatusPanel = _screen.WeaponUpgradeStatusPanel;
            WeaponBoosterButton weaponBoosterButton = _screen.WeaponBoosterButton;
            WeaponBoosterPanel weaponBoosterPanel = _screen.WeaponBoosterPanel;


            weaponBoosterButton.SetActive(false);
            weaponBoosterPanel.SetActive(false);

            await UniTask.WaitUntil(() => weaponUpgradeStatusPanel.IsInited, cancellationToken: ct);

            weaponBoosterPanel.Init(_boosterService,
                weaponUpgradeStatusPanel,
                weaponBoosterButton,
                _rewardedAdsService);
            weaponBoosterPanel.RunAsync(ct, _disposable).Forget();
        }

        public void ShowConsumableFlyToast(ConsumableData data, Vector3 toastPosition, Vector3 toastEndPosition)
        {
            ConsumableType type = data.ConsumableType;
            if (type is ConsumableType.Hidden)
            {
                return;
            }

            ConsumableCounterView counterView = _screen.GetConsumableCounter(data.Id);

            _flyingConsumables.Add(data.Id);
            _toastsService.ShowConsumableFlyToast(data.Id,
                toastPosition,
                toastEndPosition,
                () => EndFlyingConsumables(data.Id, counterView));
        }


        private void EndFlyingConsumables(ConsumablePersistenceId id, ConsumableCounterView counterView)
        {
            bool removed = _flyingConsumables.Remove(id);
            if (removed)
            {
                counterView.ChangeAmount(1, true);
            }
        }


        private void RunCraftStatus()
        {
            _craftStatusScreenPresenter = new CraftStatusScreenPresenter(_screenFactory);

            IDisposable craftStatus = _screen.CraftStatusButton.OnButtonClickAsObservable()
                .Subscribe(_ =>
                {
                    _craftCts?.CancelThenDispose();
                    _craftCts = new CancellationTokenSource();

                    _craftStatusScreenPresenter.RunAsync(ScreenOpenType.RequiredClick, _craftCts.Token).Forget();
                });
            _disposable.Add(craftStatus);

            IDisposable craftButtonActive = Observable.EveryUpdate()
                .Subscribe(_ =>
                {
                    var buildings = _world.GetObjects<CraftBuildingFlow>();
                    buildings = buildings.Where(building =>
                            building.Get<CraftBuildingView>() && building.State.Value != CraftBuildingStates.Idle)
                        .ToList();

                    _screen.CraftStatusButton.SetActive(buildings.Count > 0);
                });
            _disposable.Add(craftButtonActive);
        }


        private void RunMinimap()
        {
            _minimapScreenPresenter = new MinimapScreenPresenter(_screenFactory,
                _playerInputService,
                _cameraProvider,
                _world,
                _timeService,
                _offscreenIndicatorsService,
                _minimapIndicatorsService);

            _screen.MinimapButton.SetActive(false);

            var id = _minimapIndicatorsService.GetConsumableId();
            if (id != null)
            {
                ConsumableData data = _consumablesData.GetData(id);
                var amountProp = _consumablesService.GetAmountProp(data.Id);
                IDisposable minimapUnlock = amountProp.Where(amount => amount > 0)
                    .Subscribe(_ => { _screen.MinimapButton.SetActive(true); });
                _disposable.Add(minimapUnlock);
            }
            else
            {
                _screen.MinimapButton.SetActive(true);
            }

            IDisposable minimap = _screen.MinimapButton.OnButtonClickAsObservable()
                .Subscribe(_ =>
                {
                    _minimapCts?.CancelThenDispose();
                    _minimapCts = new CancellationTokenSource();

                    _minimapScreenPresenter.RunAsync(ScreenOpenType.RequiredClick, _minimapCts.Token).Forget();
                });
            _disposable.Add(minimap);
        }


        public void RunCombatPowerRequirementScreen()
        {
            _combatPowerRequirementScreenPresenter ??= new CombatPowerRequirementScreenPresenter(_screenFactory,
                _playerInputService,
                RunInventoryScreen);
        }


        public void ShowCombatPowerRequirementScreen(ScreenOpenType screenOpenType)
        {
            _combatPowerCts?.CancelThenDispose();
            _combatPowerCts = new CancellationTokenSource();

            _combatPowerRequirementScreenPresenter.RunAsync(screenOpenType, _combatPowerCts.Token).Forget();
        }


        private void RunAdOffer()
        {
            var adOfferData = _levelsLoadService.GetActiveLevelData().AdOfferData;
            var adOfferConfig = _configsService.Get<AdOfferConfig>(adOfferData.ConfigId);

            _adOfferScreenPresenter =
                new AdOfferScreenPresenter(_screenFactory,
                    _playerInputService,
                    _rewardedAdsService,
                    _consumablesService,
                    _consumablesData,
                    _screen,
                    _timeService,
                    adOfferData,
                    adOfferConfig,
                    _levelPersistenceService);

            _adOfferScreenPresenter.Init();

            AdOfferWorkAsync(adOfferConfig, _cts.Token).Forget();

            IDisposable adOffer = _screen.AdOfferButton.OnButtonClickAsObservable()
                .Subscribe(_ =>
                {
                    _adOfferCts?.CancelThenDispose();
                    _adOfferCts = new CancellationTokenSource();
                    _adOfferScreenPresenter.RunAsync(ScreenOpenType.RequiredClick, _adOfferCts.Token).Forget();
                });
            _disposable.Add(adOffer);
        }


        private async UniTask AdOfferWorkAsync(AdOfferConfig adOfferConfig, CancellationToken ct)
        {
            AdOfferButton adOfferButton = _screen.AdOfferButton;
            adOfferButton.TimeProgressPanel.gameObject.SetActive(false);
            adOfferButton.SetActive(false);

            int startDelay = _adOfferScreenPresenter.AdOfferPersistentComponent.Value
                ? adOfferConfig.StartDelay
                : adOfferConfig.FirstDelay;
            await UniTask.Delay(TimeSpan.FromSeconds(startDelay), cancellationToken: ct);

            while (true)
            {
                adOfferButton.TimeProgressPanel.gameObject.SetActive(false);
                adOfferButton.SetActive(false);
                float timeLeft = startDelay <= 0 ? adOfferConfig.TimeBeforeShow : 0;
                while (timeLeft > 0)
                {
                    await UniTask.Yield(ct);
                    timeLeft -= Time.deltaTime;
                }

                RewardAmountConfig reward = await _adOfferScreenPresenter.CalculateRewards(_world, ct);
                if (reward == null)
                {
                    continue;
                }

                _adOfferScreenPresenter.AdOfferPersistentComponent.Value = true;
                startDelay = 0;

                Sprite icon = _consumablesData.GetData(reward.Name).Sprite;
                adOfferButton.SetIcon(icon);
                adOfferButton.SetAmount(reward.Amount);
                adOfferButton.SetActive(true);
                timeLeft = adOfferConfig.ShowTime;
                adOfferButton.TimeProgressPanel.gameObject.SetActive(true);
                while (timeLeft > 0 && adOfferButton.IsActive())
                {
                    await UniTask.Yield(ct);
                    timeLeft -= Time.deltaTime;
                    adOfferButton.TimeProgressPanel.SetTime(timeLeft, adOfferConfig.ShowTime, true);
                }

                _adOfferScreenPresenter.RemoveRewards();
            }
        }

        private void RunPause()
        {
            _pauseScreenPresenter = new PauseScreenPresenter(_screenFactory,
                _playerInputService,
                _soundService,
                _vibrationService,
                _offscreenIndicatorsService,
                _timeService);

            IDisposable onPauseClick = _screen.PauseButton.OnButtonClickAsObservable()
                .Subscribe(_ =>
                {
                    _pauseScreenCts?.CancelThenDispose();
                    _pauseScreenCts = new CancellationTokenSource();
                    _pauseScreenPresenter.RunAsync(ScreenOpenType.RequiredClick, _pauseScreenCts.Token).Forget();
                });
            _disposable.Add(onPauseClick);
        }
    }
}