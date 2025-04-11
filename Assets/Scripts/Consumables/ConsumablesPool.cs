using Honeylab.Utils.Pool;
using UnityEngine;


namespace Honeylab.Consumables
{
    public class ConsumablesPool : KeyedPrefabPoolsStorageBehaviour<ConsumablePersistenceId>
    {
        public GameObject Pop(ConsumablePersistenceId key, bool activateOnPop = true)
        {
            IGameObjectPool pool = Get(key);
            return pool.Pop(activateOnPop);
        }


        public void Push(ConsumablePersistenceId key, GameObject go)
        {
            IGameObjectPool pool = Get(key);
            pool.Push(go);
        }
    }
}
