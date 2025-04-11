using UnityEngine;
using Zenject;


namespace Honeylab.Utils.Pool
{
    public class DiContainerInstantiator : IInstantiator
    {
        private readonly DiContainer _diContainer;


        public DiContainerInstantiator(DiContainer diContainer)
        {
            _diContainer = diContainer;
        }


        public GameObject Instantiate(GameObject prefab, Transform parent) =>
            _diContainer.InstantiatePrefab(prefab, parent);
    }
}
