using Honeylab.Consumables;
using Honeylab.Consumables.Configs;
using Honeylab.Gameplay.Analytics;
using Honeylab.Gameplay.Player;
using Honeylab.Gameplay.Ui;
using Honeylab.Gameplay.World;
using Honeylab.Sounds.Data;
using Honeylab.Utils.Configs;
using Honeylab.Utils.Extensions;
using Honeylab.Utils.Vfx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UniRx;
using UnityEngine;


namespace Honeylab.Gameplay.Weapons
{
    public class WeaponAttackTarget : WorldObjectComponentBase
    {
        [SerializeField] private ConfigIdProvider _configId;
        [SerializeField] private WorldObjectIdListProvider _weaponTypeIds;
        [SerializeField] private WeaponAttackTargetView _view;
        [SerializeField] private Health _health;

        private CompositeDisposable _disposable;
        private CancellationTokenSource _run;

        private WorldObjectFlow _flow;
        private ConsumablesService _consumables;
        private WeaponAttackTargetConfig _config;

        public string ConfigId => _configId.Id;
        public List<WorldObjectId> WeaponTypeIds => _weaponTypeIds != null ? _weaponTypeIds.Objects : null;
        private ConsumablesData _consumablesData;
        private GameplayScreenPresenter _gameplayScreenPresenter;
        private PlayerInputService _playerInputService;

        public WeaponAttackTargetView View => _view;

        private Rewards _rewards;

        public WeaponAttackTargetConfig Config => _config;

        protected override void OnInit()
        {
            _flow = GetFlow();
            _consumables = _flow.Resolve<ConsumablesService>();

            IConfigsService configs = _flow.Resolve<IConfigsService>();
            _config = configs.Get<WeaponAttackTargetConfig>(_configId.Id);
            _consumablesData = _flow.Resolve<ConsumablesData>();
            _gameplayScreenPresenter = _flow.Resolve<GameplayScreenPresenter>();

            _playerInputService = _flow.Resolve<PlayerInputService>();
        }


        protected override void OnRun()
        {
            _disposable = new CompositeDisposable();

            _rewards = new Rewards(_config.Rewards);

            IDisposable healthChanged = _health.HealthProp.Subscribe(_ => { _view.UpdateHealthView(); });
            _disposable.Add(healthChanged);

            IDisposable stateChanged = _view.OnStateChanged.Subscribe(GiveRewards);
            _disposable.Add(stateChanged);

            _view.ChangeState(_health.HealthProp.Value);
        }


        protected override void OnStop()
        {
            _disposable?.Dispose();
            _disposable = null;

            _run?.CancelThenDispose();
            _run = null;
        }


        public bool CanHit() => _health.HealthProp.Value > 0 && !_playerInputService.IsBlocked;

        public bool CanDamage(int weaponLevel) => weaponLevel >= _config.WeaponLevel - 1;


        public void Hit(WeaponFlow weapon, bool showNotEnoughLevelIfNeed = true, int damageMultiplier = 1)
        {
            if (!CanHit())
            {
                throw new Exception($"Can`t hit weapon target {_flow.Id.name}");
            }

            if (weapon.UpgradeLevel.Value < _config.WeaponLevel - 1)
            {
                _view.PlayHitReaction(false, weapon, 0, showNotEnoughLevelIfNeed);
                return;
            }

            float damage = weapon.UpgradeConfigProp.Value.Damage * damageMultiplier;

            float newHealth = _health.HealthProp.Value - damage;
            newHealth = newHealth > 0.0f ? newHealth : 0.0f;
            _health.ChangeHealth(weapon.GetAgent().GetId, newHealth);

            _view.PlayHitReaction(true, weapon, Mathf.CeilToInt(damage));
            _view.ChangeState(_health.HealthProp.Value, weapon);
        }

        public void HitFromLevel(WorldObjectFlow worldObjectFlow, float damage, VfxId vfxId = null, SoundId soundId = null)
        {
            if (!CanHit())
            {
                throw new Exception($"Can`t hit weapon target {_flow.Id.name}");
            }

            float newHealth = _health.HealthProp.Value - damage;
            newHealth = newHealth > 0.0f ? newHealth : 0.0f;
            _health.ChangeHealth(worldObjectFlow.Id, newHealth);

            _view.PlayHitReactionTest(true, Mathf.RoundToInt(damage), true, vfxId, soundId);
            _view.ChangeState(_health.HealthProp.Value);
        }


        private void GiveRewards(WeaponFlow weapon)
        {
            if (_rewards == null || weapon == null)
            {
                return;
            }

            float ratio = _health.HealthProp.Value / _health.MaxHealth;
            int stateIndex = Mathf.CeilToInt(Mathf.Lerp(0, _view.StatesCount - 1, ratio));

            Vector3 toastPosition = _flow.transform.position + Vector3.up * 2.0f;

            Vector3 toastEndPosition = weapon.GetAgent().Transform.position;

            foreach (var reward in _rewards.GetRewards)
            {
                int hitReward = stateIndex > 0 ? Mathf.CeilToInt((float)reward.MaxAmount / (_view.StatesCount - 1)) : 1;

                int rewardAmount = _health.HealthProp.Value <= 0 ?
                    reward.Amount :
                    Mathf.Min(hitReward, reward.Amount - 1);

                if (rewardAmount <= 0)
                {
                    continue;
                }

                _rewards.ChangeAmount(reward.Name, -rewardAmount);

                ConsumableData data = _consumablesData.GetData(reward.Name);

                for (int i = 0; i < rewardAmount; i++)
                {
                    _gameplayScreenPresenter.ShowConsumableFlyToast(data, toastPosition, toastEndPosition);
                    GiveAmount(reward.Name);
                }
            }
        }


        public int GetReward(string rewardName)
        {
            var reward = _rewards.GetReward(rewardName);
            return reward?.Amount ?? 0;
        }


        public IReadOnlyList<string> GetRewardNames()
        {
            return _rewards?.GetRewards.Select(it => it.Name).ToList();
        }


        private void GiveAmount(string rewardName)
        {
            ConsumableData data = _consumablesData.GetData(rewardName);
            ConsumablePersistenceId rewardId = data.Id;

            _consumables.ChangeAmount(rewardId, 1, new TransactionSource(_flow.name, TransactionType.Weapon));
        }


        private class Rewards
        {
            private List<Reward> _rewards = new List<Reward>();

            public Rewards(List<RewardAmountConfig> config)
            {
                if (config == null)
                {
                    return;
                }

                foreach (RewardAmountConfig reward in config)
                {
                    const int maxChance = 100;
                    int random = UnityEngine.Random.Range(0, maxChance);
                    int chance = reward.Chance == -1 ? maxChance : reward.Chance;

                    _rewards.Add(new Reward()
                    {
                        Name = reward.Name,
                        Amount = chance >= random ? reward.Amount : 0,
                        MaxAmount = reward.Amount
                    });
                }
            }


            public List<Reward> GetRewards => _rewards;


            public void ChangeAmount(string rewardName, int amount)
            {
                var reward = GetReward(rewardName);
                reward.Amount += amount;
            }


            public Reward GetReward(string rewardName) => GetRewards.FirstOrDefault(it => it.Name.Equals(rewardName));


            public class Reward
            {
                public string Name;
                public int Amount;
                public int MaxAmount;
                public int Chance;
            }
        }
    }
}
