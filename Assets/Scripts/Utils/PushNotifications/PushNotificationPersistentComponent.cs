using Honeylab.Utils.Persistence;
using System.Collections.Generic;


namespace Honeylab.Persistence
{
    public class PushNotificationsPersistentComponent : PersistentComponent
    {
        public Dictionary<int, int> NameHashToId { get; set; } = new Dictionary<int, int>();
    }
}
