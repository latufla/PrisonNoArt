using Honeylab.Utils.Persistence;
using System;


namespace Honeylab.Persistence
{
    [Serializable]
    public class DamageBoostPersistentComponent : PersistentComponent
    {
        public bool IsFirstDamageBoost { get; set; }
        public bool ItWasStarting { get; set; }
        public double StartBoostShowTime { get; set; }
        public double StartBoostActiveTime { get; set; }
    }
}
