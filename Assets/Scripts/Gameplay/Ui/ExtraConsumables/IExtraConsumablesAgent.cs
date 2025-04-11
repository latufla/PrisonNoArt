using Honeylab.Consumables.Configs;
using Honeylab.Gameplay.World;
using System.Collections.Generic;


namespace Honeylab.Gameplay.Ui
{
    public interface IExtraConsumablesAgent
    {
        WorldObjectId Id { get; }

        public List<RewardAmountConfig> Price { get; }

        public IExtraConsumablesPriceConfig ExtraConsumablesPriceConfig { get; }
        public IRewardedAdConsumablesAmountConfig RewardedAdConsumablesAmountConfig { get; }

        T Resolve<T>();
    }
}
