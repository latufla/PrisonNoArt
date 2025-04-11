using UnityEngine;
using Zenject;


namespace Honeylab.Utils.Pool
{
    public class ZenjectGameObjectPoolBehaviour : MonoBehaviour, IGameObjectPool, IPrewarmPool
    {
        [SerializeField] private GameObject _prefab;
        [SerializeField] private bool _useInstantiateOnly;
        [SerializeField] private int _prewarmAmount;


        private DiContainerInstantiator _instantiator;
        private GameObjectPool _pool;


        public int PrewarmAmount => _prewarmAmount;


        [Inject]
        public void Construct(DiContainer diContainer)
        {
            _instantiator = new DiContainerInstantiator(diContainer);
        }


        private void Awake()
        {
            _pool = new GameObjectPool(_instantiator, _prefab, transform, _useInstantiateOnly);
        }


        public GameObject Pop(bool activateOnPop) => _pool.Pop(activateOnPop);
        public void Push(GameObject go) => _pool.Push(go);
        public void EnsurePooledObjectCount(int count) => _pool.EnsurePooledObjectCount(count);
    }
}
