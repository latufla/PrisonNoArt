using UnityEngine;


namespace Honeylab.Gameplay.World
{
    public class WorldObjectIdProvider : MonoBehaviour
    {
        [SerializeField] private WorldObjectId _id;


        public WorldObjectId Id => _id;


        public void SetId(WorldObjectId newId)
        {
            _id = newId;
        }
    }
}
