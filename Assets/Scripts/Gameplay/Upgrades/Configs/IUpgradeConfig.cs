using Honeylab.Consumables.Configs;
using System.Collections.Generic;
using System.Linq;


namespace Honeylab.Gameplay.Upgrades
{
    public interface IUpgradeLevelConfig
    {
        public List<RewardAmountConfig> Price { get; set; }
    }


    public class UpgradeLevelConfig : IUpgradeLevelConfig
    {
        public List<RewardAmountConfig> Price { get; set; }
    }


    public interface IUpgradeItemConfig
    {
        List<IUpgradeLevelConfig> GetLevels();
    }


    public class UpgradeItemConfig : IUpgradeItemConfig
    {
        public List<UpgradeLevelConfig> Levels { get; set; }

        public List<IUpgradeLevelConfig> GetLevels() => Levels.Cast<IUpgradeLevelConfig>().ToList();
    }


    public class UpgradeConfig
    {
        public UpgradeItemConfig Upgrade { get; set; }
    }
}
