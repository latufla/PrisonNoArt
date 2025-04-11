using UnityEngine;


namespace Honeylab.Consumables
{
    public class ConsumableIdProvider : MonoBehaviour
    {
        [SerializeField] private ConsumablePersistenceId _id;


        public ConsumablePersistenceId Id => _id;
    }
}
