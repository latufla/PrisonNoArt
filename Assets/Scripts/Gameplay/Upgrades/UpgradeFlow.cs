using Honeylab.Analytics;
using Honeylab.Consumables;
using Honeylab.Gameplay.Analytics;
using Honeylab.Gameplay.World;
using Honeylab.Persistence;
using Honeylab.Utils;
using Honeylab.Utils.Analytics;
using Honeylab.Utils.Configs;
using Honeylab.Utils.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Honeylab.Gameplay.Upgrades
{
    public class UpgradeFlow : WorldObjectFlow
    {
        [SerializeField] private ConfigIdProvider _configId;

        private ConsumablesService _consumablesService;
        private LevelPersistenceService _levelPersistenceService;
        private VibrationService _vibrationsService;
        private IAnalyticsService _analyticsService;

        private UpgradeConfig _config;

        public string ConfigId => _configId.Id;
        public UpgradePersistentComponent UpgradeLevelPersistence { get; set; }


        protected override void OnInit()
        {
            _consumablesService = Resolve<ConsumablesService>();
            _levelPersistenceService = Resolve<LevelPersistenceService>();
            _vibrationsService = Resolve<VibrationService>();
            _analyticsService = Resolve<IAnalyticsService>();

            IConfigsService configs = Resolve<IConfigsService>();
            _config = configs.Get<UpgradeConfig>(_configId.Id);

            PersistentObject po = _levelPersistenceService.GetOrCreate(Id);
            UpgradeLevelPersistence = po.GetOrAdd<UpgradePersistentComponent>();
        }


        public bool CanUpgrade() => HasNextUpgrade() && HasEnoughConsumables();


        public void Upgrade(bool withdrawConsumables = false)
        {
            if (!HasNextUpgrade())
            {
                throw new Exception("Upgraded max");
            }

            if (!HasEnoughConsumables() && withdrawConsumables)
            {
                throw new Exception("Not enough consumables");
            }

            if (withdrawConsumables)
            {
                UpgradeLevelConfig config = GetLevelUpgradeConfig();
                config.Price.ForEach(it => _consumablesService.ChangeAmount(it.Name, -it.Amount, new TransactionSource(name, TransactionType.Upgrade)));
            }

            UpgradeLevelPersistence.Value += 1;

            SendAnalytics();

            _vibrationsService.Vibrate();
        }


        public void Upgrade(int toLevel, bool withdrawConsumables = false)
        {
            int fromLevel = UpgradeLevelPersistence.Value;
            if (toLevel <= fromLevel)
            {
                return;
            }

            int n = toLevel - fromLevel;
            for (int i = 0; i < n; ++i)
            {
                if (!HasNextUpgrade())
                {
                    return;
                }

                Upgrade(withdrawConsumables);
            }
        }


        public bool HasEnoughConsumables()
        {
            UpgradeLevelConfig config = GetLevelUpgradeConfig();
            return config.Price.All(it => _consumablesService.HasEnoughAmount(it.Name, it.Amount));
        }


        public UpgradeLevelConfig GetLevelUpgradeConfig() => GetLevelUpgradeConfig<UpgradeLevelConfig>(_config.Upgrade,
            UpgradeLevelPersistence.Value + 1);


        public T GetLevelUpgradeConfig<T>() where T : IUpgradeLevelConfig => GetLevelUpgradeConfig<T>(_config.Upgrade,
            UpgradeLevelPersistence.Value + 1);


        public int GetLevel() => UpgradeLevelPersistence.Value + 1;
        public int GetNextLevel() => HasNextUpgrade() ? GetLevel() + 1 : -1;
        public int GetMaxLevel() => _config.Upgrade.GetLevels().Count;


        public bool HasNextUpgrade() => HasUpgrade(_config.Upgrade, UpgradeLevelPersistence.Value + 1);


        private bool HasUpgrade(IUpgradeItemConfig config, int level)
        {
            var levels = config.GetLevels();
            return level < levels.Count;
        }


        public T GetLevelUpgradeConfig<T>(IUpgradeItemConfig config, int level)
            where T : IUpgradeLevelConfig
        {
            var levels = config.GetLevels();
            if (level >= levels.Count)
            {
                return (T)levels.LastOrDefault();
            }

            return (T)levels[level];
        }


        private void SendAnalytics()
        {
            int currentLevel = UpgradeLevelPersistence.Value;
            var msg = new Dictionary<string, object>
            {
                [AnalyticsParameters.Name] = Id.name,
                [AnalyticsParameters.Amount] = 0,
                [AnalyticsParameters.Level] = currentLevel,
            };
            _analyticsService.ReportEvent(AnalyticsKeys.Upgrade, msg);
        }
    }
}
