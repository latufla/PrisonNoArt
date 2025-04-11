using UnityEngine;


namespace Honeylab.Utils.Pool
{
    public static class KeyedGameObjectPoolsStorageExtension
    {
        public static GameObject Pop<T>(this IKeyedGameObjectPoolsStorage<T> poolsStorage,
            T key,
            bool activateOnPop = true)
        {
            IGameObjectPool pool = poolsStorage.Get(key);
            return pool.Pop(activateOnPop);
        }


        public static void Push<T>(this IKeyedGameObjectPoolsStorage<T> poolsStorage,
            T key,
            GameObject go)
        {
            IGameObjectPool pool = poolsStorage.Get(key);
            pool.Push(go);
        }


        public static void EnsurePooledObjectCount<T>(this IKeyedGameObjectPoolsStorage<T> poolsStorage,
            T key,
            int count)
        {
            IGameObjectPool pool = poolsStorage.Get(key);
            pool.EnsurePooledObjectCount(count);
        }
    }
}
