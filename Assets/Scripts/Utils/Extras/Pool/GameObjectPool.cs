using System.Collections.Generic;
using UnityEngine;


namespace Honeylab.Utils.Pool
{
    public class GameObjectPool : IGameObjectPool
    {
        private readonly Stack<GameObject> _pool = new();
        private readonly IInstantiator _instantiator;
        private readonly GameObject _prefab;
        private readonly Transform _instancesParent;
        private readonly bool _useInstantiateOnly;


        public GameObjectPool(IInstantiator instantiator,
            GameObject prefab,
            Transform instancesParent,
            bool useInstantiateOnly = false)
        {
            _instantiator = instantiator;
            _prefab = prefab;
            _instancesParent = instancesParent;
            _useInstantiateOnly = useInstantiateOnly;
        }


        public GameObject Pop(bool activateOnPop)
        {
            GameObject go;

            if (_useInstantiateOnly || _pool.Count == 0)
            {
                bool activeStateBackup = _prefab.activeSelf;
                _prefab.SetActive(false);
                go = _instantiator.Instantiate(_prefab, _instancesParent);
                _prefab.SetActive(activeStateBackup);
            }
            else
            {
                go = _pool.Pop();
            }
            go.SetActive(activateOnPop);
            return go;
        }


        public void Push(GameObject go)
        {
            if (go == null)
            {
                return;
            }

            if (_useInstantiateOnly)
            {
                Object.Destroy(go);
            }

            go.SetActive(false);
            go.transform.SetParent(_instancesParent, false);
            _pool.Push(go);
        }


        public bool HasObjectOfType<T>()
        {
            if (!_prefab.TryGetComponent(out T component))
            {
                return false;
            }

            return true;
        }


        public void EnsurePooledObjectCount(int count)
        {
            bool activeStateBackup = _prefab.activeSelf;
            _prefab.SetActive(false);
            while (_pool.Count < count)
            {
                GameObject go = _instantiator.Instantiate(_prefab, _instancesParent);
                go.SetActive(false);
                _pool.Push(go);
            }

            _prefab.SetActive(activeStateBackup);
        }
    }
}
