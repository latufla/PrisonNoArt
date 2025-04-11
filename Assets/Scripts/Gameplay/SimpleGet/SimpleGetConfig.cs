using Honeylab.Consumables.Configs;

namespace Honeylab.Gameplay
{
    public class SimpleGetConfig
    {
        public SimpleGetInfo SimpleGetInfo { get; set; }
    }

    public class SimpleGetInfo
    {
        public string Title { get; set; }
        public RewardAmountConfig RewardedAdResult { get; set; }
        public RewardAmountConfig ConsumablesResult { get; set; }
        public RewardAmountConfig ConsumablesRequired { get; set; }
    }
}
