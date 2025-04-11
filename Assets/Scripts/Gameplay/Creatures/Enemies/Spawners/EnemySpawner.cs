using Honeylab.Gameplay.Pools;
using Honeylab.Gameplay.World;
using Honeylab.Gameplay.World.Spawners;
using Honeylab.Utils.Extensions;
using Honeylab.Utils.Pool;
using System;
using UniRx;
using UnityEngine;
using Random = UnityEngine.Random;


namespace Honeylab.Gameplay.Creatures.Spawners
{
    public class EnemySpawner : WorldObjectsSpawnerBase
    {
        [SerializeField] private EnemySkinIdListProvider _skins;

        private EnemySpawnerFlow _flow;
        private CreaturesPool _pool;

        private readonly Subject<Unit> _enemyDespawnSubject = new();
        private int _maxCount = -1;


        public IObservable<Unit> OnEnemyDespawnAsObservable() => _enemyDespawnSubject.AsObservable();


        public void SetMaxCount(int maxCount)
        {
            _maxCount = maxCount;
        }


        public void ClearMaxCount()
        {
            _maxCount = -1;
        }


        protected override void OnInit()
        {
            base.OnInit();

            _flow = GetFlow<EnemySpawnerFlow>();

            GameplayPoolsService pools = _flow.Resolve<GameplayPoolsService>();
            _pool = pools.Get<CreaturesPool>();
        }


        protected override void OnRun()
        {
            _flow.NextSpawnTime = CalcNextSpawnTime(true);

            base.OnRun();
        }


        protected override bool CanSpawn(float time)
        {
            int maxInLifetimeCount = _flow.Config.MaxInLifetimeCount;
            if (maxInLifetimeCount > 0 && _flow.DiedEnemiesPersistence.Value >= maxInLifetimeCount)
            {
                return false;
            }

            int maxCount = CalcMaxCount();
            if (_flow.Objects.Count >= maxCount)
            {
                return false;
            }

            if (time < _flow.NextSpawnTime)
            {
                return false;
            }

            return true;
        }


        protected override WorldObjectId CalcNextObjectId()
        {
            int index = Random.Range(0, _flow.ObjectIds.Count);
            WorldObjectId id = _flow.ObjectIds[index];
            return id;
        }


        protected override void OnSpawn()
        {
            int maxCount = CalcMaxCount();
            if (_flow.Objects.Count >= maxCount)
            {
                _flow.IsSpawnPaused = true;
                _flow.NextSpawnTime = float.MaxValue;
            }
            else
            {
                _flow.NextSpawnTime = CalcNextSpawnTime();
            }
        }


        protected override void OnDespawn()
        {
            if (_flow.IsSpawnPaused)
            {
                _flow.IsSpawnPaused = false;
                _flow.NextSpawnTime = CalcNextSpawnTime();
            }

            _flow.DiedEnemiesPersistence.Value++;
            _enemyDespawnSubject.OnNext();
        }


        protected override WorldObjectFlow CreateObject(WorldObjectId id,
            Transform spawnPoint,
            bool activateOnPop = true)
        {
            GameObject go = _pool.Pop((CreatureId)id, activateOnPop);
            EnemyFlow enemy = go.GetComponent<EnemyFlow>();

            int skinIndex = Random.Range(0, _skins.Objects.Count);
            EnemySkinId skinId = _skins.Objects[skinIndex];
            enemy.SetArgs(new EnemyArgs
            {
                SpawnerId = _flow.Id,
                SkinId = skinId,
                SpawnPoint = spawnPoint,
                ConfigId = _flow.EnemyConfig
            });

            return enemy;
        }


        protected override void DestroyObject(WorldObjectFlow flow)
        {
            if (flow != null)
            {
                _pool.Push((CreatureId)flow.Id, flow.gameObject);
            }
        }


        private int CalcMaxCount() => _maxCount >= 0 ? _maxCount : _flow.Config.MaxCount;


        private float CalcNextSpawnTime(bool isFirstTime = false)
        {
            FloatRange range = isFirstTime ? _flow.Config.Delay : _flow.Config.Interval;
            float delay = Random.Range(range.Min, range.Max);
            return GetTime() + delay;
        }


        private void OnDrawGizmos()
        {
            var points = GetSpawnPoints();
            foreach (Transform p in points)
            {
                Gizmos.color = Color.magenta;
                Gizmos.DrawSphere(p.position, 1.0f);
            }
        }
    }
}
