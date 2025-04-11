using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Honeylab.Utils.Pool
{
    public class KeyedPrefabPoolsStorageBehaviour<T> : KeyedPrefabPoolsStorageBehaviourBase<T>, IKeyedPrewarmPool<T>
    {
        [SerializeField] private KeyedPrefabBinding<T>[] _bindings;
        [SerializeField] private bool _useInstantiateOnly;
        [SerializeField] private int _prewarmAmount;

        private KeyedGameObjectPoolsStorage<T> _keyedGameObjectPoolsStorage;


        public int PrewarmAmount => _prewarmAmount;


        private void Awake()
        {
            _keyedGameObjectPoolsStorage =
                new KeyedGameObjectPoolsStorage<T>(DefaultInstantiator.Instance,
                    transform,
                    GetPrefab,
                    _useInstantiateOnly);
        }


        public override IGameObjectPool Get(T key) => _keyedGameObjectPoolsStorage.Get(key);
        public override IEnumerable<T> GetKeys() => _bindings.Select(b => b.Key);


        private GameObject GetPrefab(T key)
        {
            var prefabBinding = _bindings.FirstOrDefault(b => b.Key.Equals(key));
            if (prefabBinding == null)
            {
                throw new KeyNotFoundException($"Can't find {key} binding");
            }

            return prefabBinding.Prefab;
        }
    }
}
