using Honeylab.Gameplay.Pools;
using Honeylab.Gameplay.World;
using Honeylab.Gameplay.World.Spawners;
using Honeylab.Persistence;
using Honeylab.Pools;
using Honeylab.Utils.Persistence;
using Honeylab.Utils.Pool;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Honeylab.Gameplay.Player
{
    public class PlayerSpawner : WorldObjectsSpawnerBase
    {
        private PlayerSpawnerFlow _flow;
        private PlayersPool _pool;
        private Transform _currentSpawnPoint;
        private LevelPersistenceService _levelPersistenceService;
        private PlayerSpawnerPersistentComponent _spawnIndexPersistence;


        protected override void OnInit()
        {
            base.OnInit();

            _flow = GetFlow<PlayerSpawnerFlow>();

            GameplayPoolsService pools = _flow.Resolve<GameplayPoolsService>();
            _pool = pools.Get<PlayersPool>();

            _levelPersistenceService = _flow.Resolve<LevelPersistenceService>();

            _spawnIndexPersistence =
                _levelPersistenceService.GetOrAddComponent<PlayerSpawnerPersistentComponent>(_flow.Id);
            _currentSpawnPoint = GetSpawnPoints()[_spawnIndexPersistence.Value];
        }


        protected override WorldObjectFlow CreateObject(WorldObjectId id,
            Transform spawnPoint,
            bool activateOnPop = true)
        {
            GameObject go = _pool.Pop(id, activateOnPop);
            WorldObjectFlow player = go.GetComponent<WorldObjectFlow>();
            return player;
        }


        public void SetSpawnPoint(Transform spawnPoint)
        {
            var points = GetSpawnPoints();
            for (int i = 0; i < points.Count; i++)
            {
                if (!spawnPoint.Equals(points[i]))
                {
                    continue;
                }

                _currentSpawnPoint = spawnPoint;
                _spawnIndexPersistence.Value = i;
            }
        }


        public override Transform GetSpawnPoint() => _currentSpawnPoint ? _currentSpawnPoint : GetSpawnPoints().First();


        protected override void OnSpawn()
        {
            if (_flow.Objects.Count > 0)
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
        }


        protected override List<Transform> GetSpawnPoints()
        {
            SpawnPoints = GetComponentsInChildren<PlayerSpawnerInteractable>()
                .Where(it => it.transform != transform)
                .Select(it => it.SpawnPoint)
                .ToList();

            return SpawnPoints;
        }


        private float CalcNextSpawnTime()
        {
            float delay = _flow.Config.Interval;
            return GetTime() + delay;
        }


        protected override bool CanSpawn(float time)
        {
            if (_flow.Objects.Count > 0)
            {
                return false;
            }

            if (time < _flow.NextSpawnTime)
            {
                return false;
            }

            return true;
        }


        protected override void DestroyObject(WorldObjectFlow flow)
        {
            if (flow != null)
            {
                _pool.Push(flow.Id, flow.gameObject);
            }
        }


        private void OnDrawGizmos()
        {
            var points = GetSpawnPoints();
            foreach (Transform p in points)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(p.position, 1.0f);
            }
        }
    }
}
