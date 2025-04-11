using Honeylab.Consumables.Configs;
using System.Collections.Generic;


namespace Honeylab.Ads
{
    public interface IRewardedAdsConsumablesConfig
    {
        List<RewardAmountConfig> RewardedAdConsumables { get; set; }
    }
}
