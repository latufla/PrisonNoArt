using UnityEngine;


namespace Honeylab.Utils.Pool
{
    public class DefaultInstantiator : IInstantiator
    {
        public static readonly DefaultInstantiator Instance = new DefaultInstantiator();


        private DefaultInstantiator() { }


        public GameObject Instantiate(GameObject prefab, Transform parent) => Object.Instantiate(prefab, parent);
    }
}
