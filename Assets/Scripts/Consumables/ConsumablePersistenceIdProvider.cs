using UnityEngine;


namespace Honeylab.Consumables
{
    public class ConsumablePersistenceIdProvider : MonoBehaviour
    {
        [SerializeField] private ConsumablePersistenceId _id;


        public ConsumablePersistenceId Id => _id;
    }
}
