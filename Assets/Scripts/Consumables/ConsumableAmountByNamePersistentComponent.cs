using Honeylab.Utils.Persistence;
using System.Collections.Generic;

namespace Honeylab.Persistence
{
    public class ConsumableAmountByNamePersistentComponent: PersistentComponent, IShouldSerialize
    {
        public Dictionary<string, int> Amount = new();
        bool IShouldSerialize.ShouldSerialize => Amount.Count > 0;
    }
}
