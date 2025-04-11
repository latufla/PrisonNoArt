using Honeylab.Consumables.Configs;


namespace Honeylab.Gameplay.SpeedUp
{
    public class SpeedUpConfig
    {
        public float RewardedAdSpeedUpTime { get; set; }

        public float ConsumablesSpeedUpTimePerAmount { get; set; }
        public RewardAmountConfig ConsumablesSpeedUpAmount { get; set; }
    }
}
