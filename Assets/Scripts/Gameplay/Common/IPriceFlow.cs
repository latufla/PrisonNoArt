using Honeylab.Consumables.Configs;
using System.Collections.Generic;


namespace Honeylab.Gameplay
{
    public interface IPriceFlow
    {
        public List<RewardAmountConfig> Price { get; }
    }
}
