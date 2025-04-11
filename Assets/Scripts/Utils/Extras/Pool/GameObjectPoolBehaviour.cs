using UnityEngine;


namespace Honeylab.Utils.Pool
{
    public class GameObjectPoolBehaviour : MonoBehaviour, IGameObjectPool, IPrewarmPool
    {
        [SerializeField] private GameObject _prefab;
        [SerializeField] private bool _useInstantiateOnly;
        [SerializeField] private int _prewarmAmount;

        private GameObjectPool _pool;


        public int PrewarmAmount => _prewarmAmount;


        private void Awake()
        {
            _pool = new GameObjectPool(DefaultInstantiator.Instance, _prefab, transform, _useInstantiateOnly);
        }


        public GameObject Pop(bool activateOnPop) => _pool.Pop(activateOnPop);
        public void Push(GameObject go) => _pool.Push(go);
        public void EnsurePooledObjectCount(int count) => _pool.EnsurePooledObjectCount(count);
    }
}
