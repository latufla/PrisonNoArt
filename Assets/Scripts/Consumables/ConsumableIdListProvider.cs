using System.Collections.Generic;
using UnityEngine;


namespace Honeylab.Consumables
{
    public class ConsumableIdListProvider : MonoBehaviour
    {
        [SerializeField] private List<ConsumablePersistenceId> _consumables;


        public List<ConsumablePersistenceId> Consumables => _consumables;
    }
}
