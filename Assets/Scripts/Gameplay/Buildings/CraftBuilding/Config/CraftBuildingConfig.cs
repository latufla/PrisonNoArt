using Honeylab.Consumables.Configs;
using Honeylab.Gameplay.SpeedUp;
using System.Collections.Generic;


namespace Honeylab.Gameplay.Buildings.Config
{
    public class CraftBuildingConfig : ISpeedUpAgentConfig
    {
        public List<RewardAmountConfig> CraftPrice { get; set; }
        public float CraftDuration { get; set; }
        public RewardAmountConfig CraftResult { get; set; }
        public bool SpeedUpHide { get; set; }
    }
}
