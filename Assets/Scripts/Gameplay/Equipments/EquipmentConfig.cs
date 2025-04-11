using Honeylab.Consumables.Configs;
using Honeylab.Gameplay.Upgrades;
using System.Collections.Generic;
using System.Linq;


namespace Honeylab.Gameplay.Equipments
{
    public class EquipmentUpgradeLevelConfig : IUpgradeLevelConfig
    {
        public List<RewardAmountConfig> Price { get; set; }
        public int Health { get; set; }
        public int Damage { get; set; }
    }


    public class EquipmentItemConfig : IUpgradeItemConfig
    {
        public List<EquipmentUpgradeLevelConfig> Levels { get; set; }
        public List<IUpgradeLevelConfig> GetLevels() => Levels.Cast<IUpgradeLevelConfig>().ToList();
    }


    public interface IEquipmentUpgradeConfig
    {
        public EquipmentItemConfig Upgrade { get; set; }
    }


    public class EquipmentConfig : IEquipmentUpgradeConfig
    {
        public EquipmentItemConfig Upgrade { get; set; }
    }
}
