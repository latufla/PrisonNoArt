using Cysharp.Threading.Tasks;
using Honeylab.Consumables.Configs;
using Honeylab.Gameplay.SpeedUp;
using Honeylab.Gameplay.Ui;
using Honeylab.Gameplay.Upgrades;
using Honeylab.Gameplay.Weapons;
using Honeylab.Gameplay.Weapons.Upgrades;
using Honeylab.Gameplay.World;
using Honeylab.Persistence;
using Honeylab.Utils;
using Honeylab.Utils.Configs;
using Honeylab.Utils.Persistence;
using Honeylab.Utils.PushNotifications;
using System;
using System.Collections.Generic;
using System.Threading;
using UniRx;
using UnityEngine;


namespace Honeylab.Gameplay.Buildings
{
    public class UpgradeBuildingFlow : WorldObjectFlow, ISpeedUpAgent, IExtraConsumablesAgent, IPriceFlow
    {
        [SerializeField] private WorldObjectIdProvider _weaponId;

        private TimeService _time;
        private WorldObjectsService _world;
        private IConfigsService _configs;
        private WeaponsData _weaponsData;

        private readonly CompositeDisposable _disposable = new();
        private CompositeDisposable _run;
        private UpgradeTimePersistentComponent _upgradeStartTime;

        public WorldObjectId WeaponId => _weaponId.Id;

        public UpgradeFlow WeaponUpgrade { get; private set; }
        public WeaponUpgradeLevelConfig WeaponUpgradeConfig { get; private set; }
        public UpgradeStatePersistentComponent State { get; private set; }

        public ISpeedUpAgentConfig SpeedUpConfig => WeaponUpgradeConfig;
        public float Duration => WeaponUpgradeConfig.Duration;
        public IReactiveProperty<double> TimeLeft { get; } = new ReactiveProperty<double>(-1.0);


        protected override void OnInit()
        {
            _time = Resolve<TimeService>();
            _world = Resolve<WorldObjectsService>();
            _configs = Resolve<IConfigsService>();
            _weaponsData = Resolve<WeaponsData>();

            LevelPersistenceService persistence = Resolve<LevelPersistenceService>();
            if (!persistence.TryGet(Id, out PersistentObject outPo) || !outPo.Has<UpgradeStatePersistentComponent>())
            {
                PersistentObject po = outPo ?? persistence.Create(Id);
                State = po.Add<UpgradeStatePersistentComponent>();
                State.Value = UpgradeBuildingStates.Idle;

                _upgradeStartTime = po.Add<UpgradeTimePersistentComponent>();
                _upgradeStartTime.Value = -1.0f;
            }
            else
            {
                State = outPo.GetOrAdd<UpgradeStatePersistentComponent>();
                _upgradeStartTime = outPo.GetOrAdd<UpgradeTimePersistentComponent>();
            }

            WeaponUpgrade = _world.GetObject<UpgradeFlow>(_weaponId.Id);

            IPushNotificationService notifications = Resolve<IPushNotificationService>();
            IDisposable work = State.ValueProperty.Pairwise()
                .Where(it => it is { Previous: UpgradeBuildingStates.Idle, Current: UpgradeBuildingStates.Work })
                .Subscribe(_ =>
                {
                    float delay = WeaponUpgradeConfig.Duration;
                    notifications.RegisterUpgradeNotification(delay);
                });
            _disposable.Add(work);
        }


        protected override void OnClear()
        {
            _disposable?.Dispose();
        }


        protected override async UniTask OnRunAsync(CancellationToken ct)
        {
            _run = new CompositeDisposable();
            IDisposable onSpeedUpgrade = RunOnUpgrade();
            _run.Add(onSpeedUpgrade);

            while (true)
            {
                await UniTask.WaitUntil(() => State.Value == UpgradeBuildingStates.Work, cancellationToken: ct);

                if (_upgradeStartTime.Value < 0.0f)
                {
                    _upgradeStartTime.Value = _time.GetUtcTime();
                }

                TimeLeft.Value =
                    WeaponUpgradeConfig.Duration - (_time.GetUtcTime() - _upgradeStartTime.Value);

                float lastTime = _time.GetRealtime();
                while (TimeLeft.Value > 0.0f)
                {
                    await UniTask.Yield(ct);

                    double timeLeft = TimeLeft.Value;
                    float deltaTime = _time.GetDeltaRealtime(lastTime);
                    timeLeft -= deltaTime;
                    TimeLeft.Value = timeLeft > 0.0f ? timeLeft : -1.0f;

                    lastTime += deltaTime;
                }

                State.Value = UpgradeBuildingStates.Done;
                _upgradeStartTime.Value = -1.0;
            }
        }


        private IDisposable RunOnUpgrade()
        {
            WeaponUpgradeConfig weaponConfig = _configs.Get<WeaponUpgradeConfig>(WeaponUpgrade.ConfigId);

            UpgradeFlow upgrade = _world.GetObject<UpgradeFlow>(_weaponId.Id);
            return upgrade.UpgradeLevelPersistence.ValueProperty.Subscribe(_ =>
            {
                if (!upgrade.HasNextUpgrade())
                {
                    WeaponUpgradeConfig = null;
                    return;
                }

                WeaponUpgradeConfig = upgrade.GetLevelUpgradeConfig<WeaponUpgradeLevelConfig>(weaponConfig.Upgrade,
                    upgrade.GetLevel());
            });
        }


        public WeaponLevelData GetNextWeaponLevelData()
        {
            if (!WeaponUpgrade.HasNextUpgrade())
            {
                int maxLevel = WeaponUpgrade.GetMaxLevel();
                WeaponData maxWeaponData = _weaponsData.GetData(WeaponId);
                WeaponLevelData maxWeaponLevelData = maxWeaponData.Levels[maxLevel - 1];
                return maxWeaponLevelData;
            }

            int nextLevel = WeaponUpgrade.GetNextLevel();
            WeaponData weaponData = _weaponsData.GetData(WeaponId);
            WeaponLevelData weaponLevelData = weaponData.Levels[nextLevel - 1];
            return weaponLevelData;
        }


        public void SpeedUp(float time)
        {
            double timeLeft = TimeLeft.Value;
            timeLeft -= time;
            TimeLeft.Value = timeLeft > 0.0f ? timeLeft : -1.0f;
        }


        public List<RewardAmountConfig> Price => WeaponUpgradeConfig?.Price;
        public IExtraConsumablesPriceConfig ExtraConsumablesPriceConfig => WeaponUpgradeConfig;
        public IRewardedAdConsumablesAmountConfig RewardedAdConsumablesAmountConfig => WeaponUpgradeConfig;
    }
}
