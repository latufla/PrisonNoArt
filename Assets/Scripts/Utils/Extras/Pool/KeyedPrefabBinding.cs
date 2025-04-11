using System;
using UnityEngine;


namespace Honeylab.Utils.Pool
{
    [Serializable]
    internal class KeyedPrefabBinding<T>
    {
        [SerializeField] private T _key;
        [SerializeField] private GameObject _prefab;


        public T Key => _key;
        public GameObject Prefab => _prefab;
    }
}
