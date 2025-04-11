using UnityEngine;


namespace Honeylab.Utils.Pool
{
    public interface IGameObjectPool
    {
        GameObject Pop(bool activateOnPop);
        void Push(GameObject go);
        void EnsurePooledObjectCount(int count);
    }
}
