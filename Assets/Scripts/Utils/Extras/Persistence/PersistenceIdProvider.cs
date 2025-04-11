using UnityEngine;


namespace Honeylab.Utils.Persistence
{
    public class PersistenceIdProvider : MonoBehaviour
    {
        [SerializeField] private PersistenceId _id;


        public PersistenceId Id => _id;
    }
}
