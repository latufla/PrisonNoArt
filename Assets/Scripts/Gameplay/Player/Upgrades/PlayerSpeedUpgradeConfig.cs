using Honeylab.Consumables.Configs;
using Honeylab.Gameplay.Upgrades;
using System.Collections.Generic;
using System.Linq;


namespace Honeylab.Gameplay.Player.Upgrades
{
    public class PlayerSpeedUpgradeLevelConfig : IUpgradeLevelConfig
    {
        public List<RewardAmountConfig> Price { get; set; }

        public float Speed { get; set; }
    }


    public class PlayerSpeedUpgradeItemConfig : IUpgradeItemConfig
    {
        public List<PlayerSpeedUpgradeLevelConfig> Levels { get; set; }
        public List<IUpgradeLevelConfig> GetLevels() => Levels.Cast<IUpgradeLevelConfig>().ToList();
    }


    public interface IPlayerSpeedUpgradeConfig
    {
        public PlayerSpeedUpgradeItemConfig Upgrade { get; set; }
    }


    public class PlayerSpeedUpgradeConfig : IPlayerSpeedUpgradeConfig
    {
        public PlayerSpeedUpgradeItemConfig Upgrade { get; set; }
    }
}
