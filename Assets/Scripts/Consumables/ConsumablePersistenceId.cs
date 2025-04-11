using Honeylab.Utils.Data;
using Honeylab.Utils.Persistence;
using UnityEngine;


namespace Honeylab.Consumables
{
    [CreateAssetMenu(fileName = Utils.Persistence.DataUtil.DefaultFileNamePrefix + nameof(ConsumablePersistenceId),
        menuName = Utils.Persistence.DataUtil.MenuNamePrefix + "Consumable Persistence ID")]
    public class ConsumablePersistenceId : PersistenceId { }
}
