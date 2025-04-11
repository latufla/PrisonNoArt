using Cysharp.Threading.Tasks;
using Honeylab.Consumables;
using Honeylab.Gameplay.Player;
using Honeylab.Gameplay.Upgrades;
using Honeylab.Gameplay.World;
using Honeylab.Persistence;
using Honeylab.Utils.Configs;
using Honeylab.Utils.Extensions;
using Honeylab.Utils.Persistence;
using System;
using System.Linq;
using System.Threading;
using UniRx;
using UnityEngine;


namespace Honeylab.Gameplay.Equipments
{
    public class EquipmentsService : IDisposable
    {
        private readonly LevelPersistenceService _levelPersistenceService;
        private readonly EquipmentsData _equipmentsData;
        private readonly IConfigsService _configsService;
        private readonly WorldObjectsService _world;
        private readonly ConsumablesService _consumablesService;
        private readonly Subject<EquipmentId> _onEquipmentLevelChange = new Subject<EquipmentId>();
        private CancellationTokenSource _cts;

        public IObservable<EquipmentId> OnEquipmentLevelChangeAsObservable() => _onEquipmentLevelChange;
        public ReactiveProperty<int> CombatPower { get; private set; } = new ReactiveProperty<int>();
        public int AttackPower { get; private set; }
        public int HealthPower { get; private set; }

        private CompositeDisposable _disposables;

        public EquipmentsService(LevelPersistenceService levelPersistenceService,
            EquipmentsData equipmentsData,
            IConfigsService configsService,
            WorldObjectsService world,
            ConsumablesService consumablesService)
        {
            _levelPersistenceService = levelPersistenceService;
            _equipmentsData = equipmentsData;
            _configsService = configsService;
            _world = world;
            _consumablesService = consumablesService;
        }


        public void Init()
        {
            foreach (EquipmentData data in _equipmentsData.GetData())
            {
                if (_levelPersistenceService.TryGetComponent(data.Id, out EquipmentLevelPersistentComponent _))
                {
                    continue;
                }

                EquipmentLevelPersistentComponent component = _levelPersistenceService.GetOrCreate(data.Id)
                    .Add<EquipmentLevelPersistentComponent>();
                component.Value = -1;
            }
        }


        public void Run()
        {
            _cts = new CancellationTokenSource();
            _disposables = new();

            CalculatePower(_cts.Token).Forget();

            OnPlayerRespawn(_cts.Token).Forget();
        }


        private async UniTask OnPlayerRespawn(CancellationToken ct)
        {
            PlayerDie playerDie = null;
            PlayerFlow player = _world.GetObjects<PlayerFlow>().FirstOrDefault();
            if (player != null)
            {
                playerDie = player.Get<PlayerDie>();
            }

            if (playerDie == null)
            {
                await UniTask.WaitUntil(() =>
                    {
                        player = _world.GetObjects<PlayerFlow>().FirstOrDefault();
                        if (player == null)
                        {
                            return false;
                        }

                        playerDie = player.Get<PlayerDie>();
                        return playerDie != null;
                    },
                    cancellationToken: ct);
            }

            IDisposable onRespawn = playerDie.OnRespawn.Subscribe(_ =>
            {
                CalculatePower(_cts.Token).Forget();
            });
            _disposables.Add(onRespawn);
        }


        public bool TryEquipmentLevelChange(string name, int level)
        {
            EquipmentData data = _equipmentsData.GetData(name);
            if (data == null)
            {
                return false;
            }

            return HasEquipment(data.Id) && TryEquipmentLevelChange(data.Id, level);
        }


        public bool TryEquipmentLevelUp(string name)
        {
            EquipmentData data = _equipmentsData.GetData(name);
            return HasEquipment(data.Id) && TryEquipmentLevelUp(data.Id);
        }


        public bool TryEquipmentLevelChange(EquipmentId id, int level)
        {
            if (!HasEquipment(id))
            {
                return false;
            }

            EquipmentLevelPersistentComponent component =
                _levelPersistenceService.GetComponent<EquipmentLevelPersistentComponent>(id);
            if (component.Value == level)
            {
                return false;
            }

            EquipmentLevelChange(id, level);
            return true;
        }


        public bool TryEquipmentLevelUp(EquipmentId id)
        {
            if (!HasEquipment(id))
            {
                return false;
            }

            EquipmentLevelPersistentComponent component =
                _levelPersistenceService.GetComponent<EquipmentLevelPersistentComponent>(id);
            int newValue = component.Value += 1;
            EquipmentLevelChange(id, newValue);
            return true;
        }


        private void EquipmentLevelChange(EquipmentId id, int newValue)
        {
            EquipmentLevelPersistentComponent component =
                _levelPersistenceService.GetComponent<EquipmentLevelPersistentComponent>(id);
            component.Value = newValue;
            CalculatePower(_cts.Token).Forget();
            _onEquipmentLevelChange.OnNext(id);
        }


        public IReadOnlyReactiveProperty<int> GetEquipmentLevel(EquipmentId id) => _levelPersistenceService
            .GetComponent<EquipmentLevelPersistentComponent>(id)
            .ValueProperty;


        public bool HasEquipment(EquipmentId id) => id != null &&
            _levelPersistenceService.TryGetComponent(id, out EquipmentLevelPersistentComponent _);


        public bool HasEnoughConsumables(EquipmentConfig config, int level)
        {
            EquipmentUpgradeLevelConfig upgradeConfig = GetUpgradeLevelConfig(config.Upgrade, level);
            return upgradeConfig != null &&
                upgradeConfig.Price.All(it => _consumablesService.HasEnoughAmount(it.Name, it.Amount));
        }


        public EquipmentUpgradeLevelConfig GetUpgradeLevelConfig(IUpgradeItemConfig config, int level)
        {
            var levels = config.GetLevels();
            if (level >= levels.Count)
            {
                return null;
            }

            return (EquipmentUpgradeLevelConfig)levels[level];
        }


        private async UniTask CalculatePower(CancellationToken ct)
        {
            Health playerHealth = null;
            PlayerFlow player = _world.GetObjects<PlayerFlow>().FirstOrDefault();
            if (player != null)
            {
                playerHealth = player.Get<Health>();
            }

            if (playerHealth == null)
            {
                await UniTask.WaitUntil(() =>
                    {
                        player = _world.GetObjects<PlayerFlow>().FirstOrDefault();
                        if (player == null)
                        {
                            return false;
                        }

                        playerHealth = player.Get<Health>();
                        return playerHealth != null;
                    },
                    cancellationToken: ct);
            }

            int attackSum = 0;
            int healthSum = 0;
            foreach (EquipmentData data in _equipmentsData.GetData())
            {
                EquipmentId id = data.Id;
                EquipmentConfig config = _configsService.Get<EquipmentConfig>(data.ConfigId);
                int level = GetEquipmentLevel(id).Value;
                var levels = config.Upgrade.GetLevels();
                EquipmentUpgradeLevelConfig upgradeLevelConfig = null;
                if (level >= 0 && level < levels.Count)
                {
                    upgradeLevelConfig = (EquipmentUpgradeLevelConfig)levels[level];
                }

                if (upgradeLevelConfig == null)
                {
                    continue;
                }

                attackSum += upgradeLevelConfig.Damage;
                healthSum += upgradeLevelConfig.Health;
            }


            AttackPower = attackSum;
            HealthPower = Mathf.CeilToInt(healthSum + playerHealth.Config.Health);
            CombatPower.Value = Mathf.CeilToInt((AttackPower + HealthPower) * 2.5f);

            playerHealth.SetMaxHealth(HealthPower);
            playerHealth.HealthProp.Value = playerHealth.MaxHealth;
        }


        public void Dispose()
        {
            _cts?.CancelThenDispose();
            _cts = null;
            _disposables?.Dispose();
            _disposables = null;
        }
    }
}
