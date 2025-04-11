using System;
using UnityEngine;


namespace Honeylab.Utils.Pool
{
    public static class GameObjectPoolExtension
    {
        public static T PopWithComponent<T>(this IGameObjectPool pool, bool activateOnPop)
        {
            GameObject go = pool.Pop(activateOnPop);
            if (!go.TryGetComponent(out T component))
            {
                throw new InvalidOperationException();
            }

            return component;
        }
    }
}
