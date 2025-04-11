using System;
using System.Collections.Generic;
using UnityEngine;


namespace Honeylab.Utils.Pool
{
    public class KeyedGameObjectPoolsStorage<T> : IKeyedGameObjectPoolsStorage<T>
    {
        private readonly Dictionary<T, GameObjectPool> _pools = new Dictionary<T, GameObjectPool>();
        private readonly IInstantiator _instantiator;
        private readonly Transform _instancesParent;
        private readonly Func<T, GameObject> _prefabFunc;
        private readonly bool _useInstantiateOnly;


        public KeyedGameObjectPoolsStorage(IInstantiator instantiator,
            Transform instancesParent,
            Func<T, GameObject> prefabFunc,
            bool useInstantiateOnly = false)
        {
            _instantiator = instantiator;
            _instancesParent = instancesParent;
            _prefabFunc = prefabFunc;
            _useInstantiateOnly = useInstantiateOnly;
        }


        public IGameObjectPool Get(T key)
        {
            if (_pools.TryGetValue(key, out GameObjectPool pool))
            {
                return pool;
            }

            GameObject prefab = _prefabFunc(key);
            pool = new GameObjectPool(_instantiator, prefab, _instancesParent, _useInstantiateOnly);
            _pools.Add(key, pool);
            return pool;
        }
    }
}
