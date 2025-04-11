using Cysharp.Threading.Tasks;
using Honeylab.Utils.Pool;
using System.Linq;
using System.Threading;


namespace Honeylab.Gameplay.Pools
{
    public class PrewarmUtility
    {
        private readonly int _instantiatesPerFrame;
        private int _alreadyInstantiatedCount;


        public PrewarmUtility(int instantiatesPerFrame)
        {
            _instantiatesPerFrame = instantiatesPerFrame;
        }


        public async UniTask PrewarmAsync(IPrewarmPool pool, int prewarmCount, CancellationToken ct)
        {
            for (int i = 0; i < prewarmCount; i++)
            {
                pool.EnsurePooledObjectCount(i + 1);
                if ((_alreadyInstantiatedCount + i + 1) % _instantiatesPerFrame == 0)
                {
                    await UniTask.Yield(ct);
                }
            }

            _alreadyInstantiatedCount += prewarmCount;
        }


        public async UniTask PrewarmAsync(IGameObjectPool pool, int prewarmCount, CancellationToken ct)
        {
            for (int i = 0; i < prewarmCount; i++)
            {
                pool.EnsurePooledObjectCount(i + 1);
                if ((_alreadyInstantiatedCount + i + 1) % _instantiatesPerFrame == 0)
                {
                    await UniTask.Yield(ct);
                }
            }

            _alreadyInstantiatedCount += prewarmCount;
        }


        public async UniTask PrewarmAsync<T>(IKeyedPrewarmPool<T> poolsStorage,
            int prewarmCountPerKey,
            CancellationToken ct)
        {
            var consumablePools = poolsStorage.GetKeys()
                .Select(poolsStorage.Get);

            foreach (IGameObjectPool pool in consumablePools)
            {
                await PrewarmAsync(pool, prewarmCountPerKey, ct);
            }
        }
    }
}
