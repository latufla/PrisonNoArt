using System.Linq;
using UnityEngine;
using Zenject;

namespace Honeylab.Utils.Pool
{
    public class ZenjectTypedPrefabPoolsStorageBehaviour : MonoBehaviour, IPrewarmPool
    {
        [SerializeField] private GameObject[] _bindings;
        [SerializeField] private bool _useInstantiateOnly;
        [SerializeField] private int _prewarmAmount;

        private TypedGameObjectPoolsStorage _typesGameObjectPoolsStorage;

        public int PrewarmAmount => _prewarmAmount;

        [Inject]
        public void Construct(DiContainer diContainer)
        {
            DiContainerInstantiator instantiator = new(diContainer);
            _typesGameObjectPoolsStorage =
                new TypedGameObjectPoolsStorage(instantiator, transform, _bindings.ToList(), _useInstantiateOnly);
        }


        public void EnsurePooledObjectCount(int count)
        {
            _typesGameObjectPoolsStorage.EnsurePooledObjectCount(count);
        }


        public IGameObjectPool Get<T>() => _typesGameObjectPoolsStorage.Get<T>();
    }
}
