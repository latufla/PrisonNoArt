using Honeylab.Consumables.Configs;
using Honeylab.Gameplay.Config;
using Honeylab.Gameplay.SpeedUp;
using System.Collections.Generic;


namespace Honeylab.Gameplay.Buildings.Config
{
    public class UnlockBuildingConfig : ISpeedUpAgentConfig
    {
        public float UnlockDuration { get; set; }
        public List<RewardAmountConfig> UnlockPrice { get; set; }
        public bool SpeedUpHide { get; set; }
    }
}
