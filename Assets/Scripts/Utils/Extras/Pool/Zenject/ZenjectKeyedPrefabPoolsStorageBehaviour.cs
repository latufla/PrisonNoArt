using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;


namespace Honeylab.Utils.Pool
{
    public class ZenjectKeyedPrefabPoolsStorageBehaviour<T> : KeyedPrefabPoolsStorageBehaviourBase<T>
    {
        [SerializeField] private KeyedPrefabBinding<T>[] _bindings;
        [SerializeField] private bool _useInstantiateOnly;

        private KeyedGameObjectPoolsStorage<T> _keyedGameObjectPoolsStorage;


        [Inject]
        public void Construct(DiContainer diContainer)
        {
            DiContainerInstantiator instantiator = new(diContainer);
            _keyedGameObjectPoolsStorage =
                new KeyedGameObjectPoolsStorage<T>(instantiator, transform, GetPrefab, _useInstantiateOnly);
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
