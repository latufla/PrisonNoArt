using Honeylab.Utils.Persistence;
using System.Collections.Generic;


namespace Honeylab.Persistence
{
    public class UnlockBuildingConsumablesPersistentComponent : PersistentComponent
    {
        public List<int> Amounts = new();
    }
}
