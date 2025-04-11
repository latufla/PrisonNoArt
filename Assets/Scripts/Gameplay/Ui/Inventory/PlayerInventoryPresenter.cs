using Cysharp.Threading.Tasks;
using Honeylab.Consumables;
using Honeylab.Gameplay.Equipments;
using Honeylab.Gameplay.Player;
using Honeylab.Gameplay.Weapons;
using Honeylab.Gameplay.World;
using Honeylab.Sounds;
using Honeylab.Utils.Configs;
using Honeylab.Utils.Extensions;
using Honeylab.Utils.OffscreenTargetIndicators;
using System;
using System.Threading;
using UniRx;
using UnityEngine;

namespace Honeylab.Gameplay.Ui.Inventory
{
    public class PlayerInventoryPresenter : ScreenPresenterBase<PlayerInventoryScreen>
    {
        private readonly PlayerInputService _playerInputService;
        private readonly WeaponsData _weaponsData;
        private readonly ConsumablesData _consumablesData;
        private readonly ConsumablesService _consumablesService;
        private readonly EquipmentsService _equipmentsService;
        private readonly EquipmentsData _equipmentsData;
        private readonly OffscreenIndicatorsService _offscreenIndicatorsService;
        private readonly PlayerInventoryService _playerInventoryService;
        private readonly IConfigsService _configsService;
        private readonly SoundService _soundService;
        private Action<ConsumableData, ScreenOpenType> _findsScreen;

        private WorldObjectId _weaponTypeId;
        private CompositeDisposable _disposable;
        private CancellationTokenSource _cts;

        private Subject<EquipmentId> _onEquipmentUpgradeAvailable = new Subject<EquipmentId>();
        private Subject<bool> _onAnyEquipmentUpgradeAvailable = new Subject<bool>();
        public IObservable<EquipmentId> OnEquipmentUpgradeAvailableAsObservable() => _onEquipmentUpgradeAvailable.AsObservable();
        public IObservable<bool> OnAnyEquipmentUpgradeAvailableAsObservable() => _onAnyEquipmentUpgradeAvailable.AsObservable();


        public PlayerInventoryPresenter(ScreenFactory factory,
            PlayerInputService playerInputService,
            WeaponsData weaponsData,
            ConsumablesData consumablesData,
            ConsumablesService consumablesService,
            OffscreenIndicatorsService offscreenIndicatorsService,
            PlayerInventoryService playerInventoryService,
            EquipmentsService equipmentsService,
            EquipmentsData equipmentsData,
            IConfigsService configsService,
            SoundService soundService) : base(factory)
        {
            _playerInputService = playerInputService;
            _weaponsData = weaponsData;
            _consumablesData = consumablesData;
            _consumablesService = consumablesService;
            _offscreenIndicatorsService = offscreenIndicatorsService;
            _playerInventoryService = playerInventoryService;
            _equipmentsService = equipmentsService;
            _equipmentsData = equipmentsData;
            _configsService = configsService;
            _soundService = soundService;
        }


        public void Init(WorldObjectId weaponTypeId)
        {
            if (_cts != null)
            {
                return;
            }

            _weaponTypeId = weaponTypeId;

            _cts = new CancellationTokenSource();
            RunNotifications(_cts.Token).Forget();
        }


        protected override void OnRun(CancellationToken ct)
        {
            _disposable = new CompositeDisposable();

            Screen.Init(_playerInputService,
                _weaponsData,
                _consumablesData,
                _consumablesService,
                _playerInventoryService,
                _equipmentsService,
                _equipmentsData,
                _configsService,
                _soundService,
                _offscreenIndicatorsService);

            WeaponInfo weaponInfo = _playerInventoryService.GetWeaponUpgradeInfo(_weaponTypeId);

            Screen.Run(weaponInfo);

            IDisposable onEquipmentUpgradeAvailable = Screen.PlayerInventoryPanel.OnEquipmentUpgradeAvailableAsObservable()
                .Subscribe(id =>
                {
                    _onEquipmentUpgradeAvailable.OnNext(id);
                });
            _disposable.Add(onEquipmentUpgradeAvailable);
        }


        private async UniTask RunNotifications(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                await UniTask.Yield(ct);
                bool hasEnoughAny = false;
                foreach (EquipmentData data in _equipmentsData.GetData())
                {
                    int level = _equipmentsService.GetEquipmentLevel(data.Id).Value;
                    EquipmentConfig config = _configsService.Get<EquipmentConfig>(data.ConfigId);
                    bool hasEnough = level >= 0 && _equipmentsService.HasEnoughConsumables(config, level + 1);
                    if (IsRunning)
                    {
                        Screen.PlayerInventoryPanel.EquipmentsNotificationActive(hasEnough, data.Id);
                    }

                    if (hasEnough)
                    {
                        hasEnoughAny = true;
                    }
                }

                _onAnyEquipmentUpgradeAvailable.OnNext(hasEnoughAny);
            }
        }


        protected override void OnStop()
        {
            _disposable?.Dispose();
            _disposable = null;
        }

        internal void Clear()
        {
            _cts?.CancelThenDispose();
            _cts = null;

            _disposable?.Dispose();
            _disposable = null;
        }
    }
}
