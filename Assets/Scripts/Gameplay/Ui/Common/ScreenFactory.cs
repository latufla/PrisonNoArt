using Honeylab.Gameplay.Pools;
using Honeylab.Pools;
using Honeylab.Utils.Pool;
using UnityEngine;

namespace Honeylab.Gameplay.Ui
{
    public class ScreenFactory
    {
        private readonly GameplayPoolsService _pools;
        private readonly RectTransform _parent;

        public ScreenFactory(GameplayPoolsService pools, RectTransform parent)
        {
            _pools = pools;
            _parent = parent;
        }


        public virtual T Create<T>() where T: ScreenBase
        {
            IGameObjectPool pool = GetPool<T>();
            GameObject screen = pool.Pop(false);
            screen.transform.SetParent(_parent, false);
            return screen.GetComponent<T>();
        }


        public virtual void Destroy<T>(T screen) where T : ScreenBase
        {
            IGameObjectPool pool = GetPool<T>();
            pool.Push(screen.gameObject);
        }


        public IGameObjectPool GetPool<T>() => _pools.Get<ScreensPool>().Get<T>();
    }
}
