using Honeylab.Consumables.Configs;
using System.Collections.Generic;


namespace Honeylab.Gameplay.Ui
{
    public class ExtraConsumablePriceConfig
    {
        public RewardAmountConfig Consumable { get; set; }
        public RewardAmountConfig Price { get; set; }
    }

    public interface IExtraConsumablesPriceConfig
    {
        public List<ExtraConsumablePriceConfig> ConsumablePrices { get; set; }
    }
}
