using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Honeylab.Utils.Pool
{
    public class TypedGameObjectPoolsStorage
    {
        private readonly List<GameObjectPool> _pools = new List<GameObjectPool>();
        private readonly IInstantiator _instantiator;
        private readonly Transform _instancesParent;
        private readonly bool _useInstantiateOnly;
        private readonly List<GameObject> _prefabs = new List<GameObject>();

        public TypedGameObjectPoolsStorage(IInstantiator instantiator,
            Transform instancesParent,
            List<GameObject> prefabs,
            bool useInstantiateOnly = false)
        {
            _instantiator = instantiator;
            _instancesParent = instancesParent;
            _prefabs = prefabs;
            _useInstantiateOnly = useInstantiateOnly;

            foreach (var prefab in _prefabs)
            {
                CreatePool(prefab);
            }
        }


        public IGameObjectPool Get<T>()
        {
            var pool = _pools.FirstOrDefault(it => it.HasObjectOfType<T>());
            if (pool != null)
            {
                return pool;
            }
            return CreatePool<T>();
        }


        private IGameObjectPool CreatePool<T>()
        {
            GameObject prefab = _prefabs.First(it => it.TryGetComponent(out T component));
            if (prefab == null)
            {
                return null;
            }
            return CreatePool(prefab);
        }


        private IGameObjectPool CreatePool(GameObject prefab)
        {
            var pool = new GameObjectPool(_instantiator, prefab, _instancesParent, _useInstantiateOnly);
            _pools.Add(pool);
            return pool;
        }


        public void EnsurePooledObjectCount(int count)
        {
            foreach (GameObjectPool pool in _pools)
            {
                pool.EnsurePooledObjectCount(count);
            }
        }
    }
}
