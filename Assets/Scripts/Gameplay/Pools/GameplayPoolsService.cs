using Cysharp.Threading.Tasks;
using Honeylab.Consumables;
using Honeylab.Gameplay.Creatures;
using Honeylab.Gameplay.World;
using Honeylab.Utils.Pool;
using Honeylab.Utils.Vfx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;


namespace Honeylab.Gameplay.Pools
{
    public class GameplayPoolsService : MonoBehaviour
    {
        [SerializeField] private int _prewarmInstantiatesPerFrame = 10;

        private List<IPrewarmPool> _pools;
        private List<IKeyedPrewarmPool<WorldObjectId>> _worldObjectPools;
        private List<IKeyedPrewarmPool<ConsumablePersistenceId>> _consumableObjectPools;

        private IKeyedPrewarmPool<CreatureId> _creaturesPool;
        private IKeyedPrewarmPool<VfxId> _vfxPool;
        private IKeyedPrewarmPool<EnemySkinId> _enemySkinsPool;


        public async UniTask InitAsync(CancellationToken ct)
        {
            var pools = GetPools();

            PrewarmUtility prewarmUtility = new(_prewarmInstantiatesPerFrame);
            foreach (IPrewarmPool pool in pools)
            {
                await prewarmUtility.PrewarmAsync(pool, pool.PrewarmAmount, ct);
            }

            var worldObjectPools = GetWorldObjectPools();
            foreach (var pool in worldObjectPools)
            {
                await prewarmUtility.PrewarmAsync(pool, pool.PrewarmAmount, ct);
            }

            var consumablesPools = GetConsumablesObjectPools();
            foreach (var pool in consumablesPools)
            {
                await prewarmUtility.PrewarmAsync(pool, pool.PrewarmAmount, ct);
            }

            var creaturesPool = GetCreaturesPool();
            if (creaturesPool != null)
            {
                await prewarmUtility.PrewarmAsync(creaturesPool, creaturesPool.PrewarmAmount, ct);
            }

            var vfxPool = GetVfxPool();
            if (vfxPool != null)
            {
                await prewarmUtility.PrewarmAsync(vfxPool, vfxPool.PrewarmAmount, ct);
            }

            var enemySkinsPool = GetEnemySkinsPool();
            if (enemySkinsPool != null)
            {
                await prewarmUtility.PrewarmAsync(enemySkinsPool, enemySkinsPool.PrewarmAmount, ct);
            }
        }


        public T Get<T>()
        {
            T pool = GetPools().OfType<T>().FirstOrDefault();
            if (pool != null)
            {
                return pool;
            }

            pool = GetWorldObjectPools().OfType<T>().FirstOrDefault();
            if (pool != null)
            {
                return pool;
            }

            pool = GetConsumablesObjectPools().OfType<T>().FirstOrDefault();
            if (pool != null)
            {
                return pool;
            }

            if (GetCreaturesPool() is T creaturesPool)
            {
                return creaturesPool;
            }

            if (GetVfxPool() is T vfxPool)
            {
                return vfxPool;
            }

            if (GetEnemySkinsPool() is T enemySkinsPool)
            {
                return enemySkinsPool;
            }

            throw new Exception($"No pool with type {typeof(T)}");
        }


        private List<IPrewarmPool> GetPools()
        {
            _pools ??= GetComponentsInChildren<IPrewarmPool>().ToList();
            return _pools;
        }


        private List<IKeyedPrewarmPool<WorldObjectId>> GetWorldObjectPools()
        {
            _worldObjectPools ??= GetComponentsInChildren<IKeyedPrewarmPool<WorldObjectId>>().ToList();
            return _worldObjectPools;
        }

        private List<IKeyedPrewarmPool<ConsumablePersistenceId>> GetConsumablesObjectPools()
        {
            _consumableObjectPools ??= GetComponentsInChildren<IKeyedPrewarmPool<ConsumablePersistenceId>>().ToList();
            return _consumableObjectPools;
        }


        private IKeyedPrewarmPool<CreatureId> GetCreaturesPool()
        {
            _creaturesPool ??= GetComponentInChildren<IKeyedPrewarmPool<CreatureId>>();
            return _creaturesPool;
        }


        private IKeyedPrewarmPool<VfxId> GetVfxPool()
        {
            _vfxPool ??= GetComponentInChildren<IKeyedPrewarmPool<VfxId>>();
            return _vfxPool;
        }


        private IKeyedPrewarmPool<EnemySkinId> GetEnemySkinsPool()
        {
            _enemySkinsPool ??= GetComponentInChildren<IKeyedPrewarmPool<EnemySkinId>>();
            return _enemySkinsPool;
        }
    }
}
