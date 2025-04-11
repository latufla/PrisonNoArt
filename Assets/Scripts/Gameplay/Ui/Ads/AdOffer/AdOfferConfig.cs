using Honeylab.Consumables.Configs;
using System.Collections.Generic;

namespace Honeylab.Gameplay
{
    public class AdOfferConfig
    {
        public int FirstDelay { get; set; }
        public int StartDelay { get; set; }
        public int TimeBeforeShow { get; set; }
        public int ShowTime { get; set; }
        public List<AdOfferInfoConfig> AdOfferInfo { get; set; }
    }


    public class AdOfferInfoConfig
    {
        public int WeaponLevel { get; set; }
        public List<List<RewardAmountConfig>> Rewards { get; set; }
    }
}
