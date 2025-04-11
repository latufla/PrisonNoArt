using Honeylab.Consumables.Configs;
using System.Collections.Generic;


namespace Honeylab.Gameplay.Weapons
{
    public class WeaponAttackTargetConfig
    {
        public int WeaponLevel { get; set; }
        public List<RewardAmountConfig> Rewards { get; set; }
    }
}
