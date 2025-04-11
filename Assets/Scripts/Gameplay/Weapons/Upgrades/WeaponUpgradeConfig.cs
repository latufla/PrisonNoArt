using Honeylab.Ads;
using Honeylab.Consumables.Configs;
using Honeylab.Gameplay.SpeedUp;
using Honeylab.Gameplay.Ui;
using Honeylab.Gameplay.Upgrades;
using System.Collections.Generic;
using System.Linq;


namespace Honeylab.Gameplay.Weapons.Upgrades
{
    public class WeaponUpgradeLevelConfig : IUpgradeLevelConfig, IRewardedAdsConsumablesConfig, ISpeedUpAgentConfig,
        IExtraConsumablesPriceConfig, IRewardedAdConsumablesAmountConfig
    {
        public List<RewardAmountConfig> Price { get; set; }

        public float SearchRadius { get; set; }
        public float AttackRadius { get; set; }
        public float AttackAngle { get; set; }
        public float AttackWidth { get; set; }

        public float Duration { get; set; }
        public float Damage { get; set; }
        public float KnockbackPower { get; set; }
        public float KnockbackDuration { get; set; }
        public float ShakePower { get; set; }
        public float ShakeDuration { get; set; }
        public bool SpeedUpHide { get; set; }
        public int Chance { get; set; }

        public List<RewardAmountConfig> RewardedAdConsumables { get; set; }

        public List<ExtraConsumablePriceConfig> ConsumablePrices { get; set; }
        public List<RewardAmountConfig> RewardedAdConsumablesAmounts { get; set; }
    }


    public class WeaponUpgradeItemConfig : IUpgradeItemConfig
    {
        public List<WeaponUpgradeLevelConfig> Levels { get; set; }
        public List<IUpgradeLevelConfig> GetLevels() => Levels.Cast<IUpgradeLevelConfig>().ToList();
    }


    public interface IWeaponUpgradeConfig
    {
        public WeaponUpgradeItemConfig Upgrade { get; set; }
    }


    public class WeaponUpgradeConfig : IWeaponUpgradeConfig
    {
        public float RotationSpeed { get; set; }
        public WeaponUpgradeItemConfig Upgrade { get; set; }
    }
}
