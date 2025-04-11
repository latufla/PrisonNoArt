using Honeylab.Consumables.Configs;
using System.Collections.Generic;


namespace Honeylab.Ads
{
    public class RewardedAdsConsumablesConfig : IRewardedAdsConsumablesConfig
    {
        public List<RewardAmountConfig> RewardedAdConsumables { get; set; }
    }
}
