using Cysharp.Threading.Tasks;
using Honeylab.Ads;
using Honeylab.Consumables;
using Honeylab.Consumables.Configs;
using Honeylab.Gameplay.Player;
using Honeylab.Gameplay.Upgrades;
using Honeylab.Gameplay.Weapons;
using Honeylab.Gameplay.World;
using Honeylab.Persistence;
using Honeylab.Utils;
using Honeylab.Utils.Extensions;
using Honeylab.Utils.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using Random = System.Random;


namespace Honeylab.Gameplay.Ui.AdOffer
{
    public class AdOfferScreenPresenter : ScreenPresenterBase<AdOfferScreen>
    {
        private readonly PlayerInputService _playerInputService;
        private readonly RewardedAdsService _rewardedAdsService;
        private readonly ConsumablesService _consumablesService;
        private readonly ConsumablesData _consumablesData;
        private readonly TimeService _timeService;
        private readonly GameplayScreen _gameplayScreen;
        private readonly LevelPersistenceService _persistenceService;
        private IDisposable _blockInput;
        private RewardAmountConfig _reward;

        public AdOfferData AdOfferData { get; private set; }
        public AdOfferConfig AdOfferConfig { get; private set; }
        public AdOfferPersistentComponent AdOfferPersistentComponent { get; private set; }

        public AdOfferScreenPresenter(ScreenFactory factory,
            PlayerInputService playerInputService,
            RewardedAdsService rewardedAdsService,
            ConsumablesService consumablesService,
            ConsumablesData consumablesData,
            GameplayScreen gameplayScreen,
            TimeService timeService,
            AdOfferData data,
            AdOfferConfig config,
            LevelPersistenceService levelPersistenceService) : base(factory)
        {
            _playerInputService = playerInputService;
            _rewardedAdsService = rewardedAdsService;
            _consumablesService = consumablesService;
            _consumablesData = consumablesData;
            _timeService = timeService;
            _gameplayScreen = gameplayScreen;
            AdOfferData = data;
            AdOfferConfig = config;
            _persistenceService = levelPersistenceService;
        }


        public void Init()
        {
            if (!_persistenceService.TryGet(AdOfferData.AdOfferId, out PersistentObject adOfferPo) ||
                !adOfferPo.Has<AdOfferPersistentComponent>())
            {
                PersistentObject po = adOfferPo ?? _persistenceService.Create(AdOfferData.AdOfferId);
                AdOfferPersistentComponent = po.Add<AdOfferPersistentComponent>();
            }
            else
            {
                AdOfferPersistentComponent = adOfferPo.GetOrAdd<AdOfferPersistentComponent>();
            }
        }


        protected override void OnRun(CancellationToken ct)
        {
            _blockInput = _playerInputService.BlockInput();
            _timeService.Pause();
            Screen.Init(_rewardedAdsService,
                _consumablesData,
                _consumablesService,
                AdOfferData.AdOfferId,
                _timeService);

            if (_reward == null)
            {
                throw new Exception("rewards not found");
            }

            ConsumablePersistenceId id = _consumablesData.GetData(_reward.Name).Id;
            Sprite icon = AdOfferData.GetIcon(id);
            if (icon == null)
            {
                icon = AdOfferData.GetIconFirst();
            }

            Screen.Run(_reward, icon, RemoveRewards);
        }


        public void RemoveRewards()
        {
            _reward = null;
        }


        public async UniTask<RewardAmountConfig> CalculateRewards(WorldObjectsService world, CancellationToken ct)
        {
            int weaponLevel = await GetWeaponLevelAsync(world, ct);

            if (weaponLevel <= 0)
            {
                return null;
            }

            Random rnd = new();
            int chance = rnd.Next(0, 100);
            AdOfferInfoConfig adOfferInfoConfig =
                AdOfferConfig.AdOfferInfo.FirstOrDefault(it => it.WeaponLevel == weaponLevel);

            if (adOfferInfoConfig == null)
            {
                return null;
            }

            switch (chance)
            {
                case > 90:
                {
                    const int index = 0;
                    var rewards = adOfferInfoConfig.Rewards[index];
                    _reward = new RewardAmountConfig
                    {
                        Name = rewards.First().Name,
                        Amount = rewards.First().Amount
                    };
                    break;
                }
                case > 80:
                {
                    const int index = 1;
                    var rewards = adOfferInfoConfig.Rewards[index];
                    _reward = new RewardAmountConfig
                    {
                        Name = rewards.First().Name,
                        Amount = rewards.First().Amount
                    };
                    break;
                }
                default:
                {
                    const int index = 2;
                    var rewards = adOfferInfoConfig.Rewards[index];
                    var result = rewards.ToList();
                    result.Shuffle(rnd);
                    _reward = new RewardAmountConfig()
                    {
                        Name = result.First().Name,
                        Amount = result.First().Amount
                    };
                    break;
                }
            }

            if (_reward.Amount <= 0)
            {
                _reward = await CalculateRewards(world, ct);
            }

            return _reward;
        }


        private async UniTask<int> GetWeaponLevelAsync(WorldObjectsService world, CancellationToken ct)
        {
            WorldObjectId weaponId = null;
            UpgradeFlow upgrade = null;
            WeaponAgent weaponAgent = null;
            await UniTask.WaitUntil(() =>
                {
                    PlayerFlow player = world.GetObjects<PlayerFlow>().FirstOrDefault();
                    if (player == null)
                    {
                        return false;
                    }

                    weaponAgent = player.Get<WeaponAgent>();
                    if (weaponAgent == null)
                    {
                        return false;
                    }

                    weaponId = weaponAgent.GetWeaponByTypeFirstOrDefault(AdOfferData.WeaponTypeId);
                    if (weaponId == null)
                    {
                        return false;
                    }

                    upgrade = world.GetObject<UpgradeFlow>(weaponId);
                    return upgrade != null;
                },
                cancellationToken: ct);

            return upgrade.UpgradeLevelPersistence.Value + 1;
        }


        protected override void OnStop()
        {
            Screen.Clear();
            _timeService.Resume();

            _blockInput?.Dispose();
            _blockInput = null;

            if (_reward == null)
            {
                _gameplayScreen.AdOfferButton.SetActive(false);
            }
        }
    }
}
