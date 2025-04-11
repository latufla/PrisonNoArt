using System.Collections.Generic;
using UnityEngine;


namespace Honeylab.Utils.Pool
{
    public abstract class KeyedPrefabPoolsStorageBehaviourBase<T> : MonoBehaviour, IKeyedGameObjectPoolsStorage<T>
    {
        public abstract IGameObjectPool Get(T key);
        public abstract IEnumerable<T> GetKeys();
    }
}
