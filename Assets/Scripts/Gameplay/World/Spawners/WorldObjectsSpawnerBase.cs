using Cysharp.Threading.Tasks;
using Honeylab.Utils.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;


namespace Honeylab.Gameplay.World.Spawners
{
    public abstract class WorldObjectsSpawnerBase : WorldObjectComponentBase
    {
        private WorldObjectsSpawnerFlow _spawnerFlow;
        private WorldObjectsService _world;
        protected List<Transform> SpawnPoints;

        private CancellationTokenSource _run;


        protected override void OnInit()
        {
            _spawnerFlow = GetFlow<WorldObjectsSpawnerFlow>();
            _world = _spawnerFlow.Resolve<WorldObjectsService>();
        }


        protected override void OnRun()
        {
            _run = new CancellationTokenSource();
            OnRunAsync(_run.Token).Forget();
        }


        protected override void OnStop()
        {
            _run?.CancelThenDispose();
            _run = null;
        }


        private async UniTask OnRunAsync(CancellationToken ct)
        {
            while (true)
            {
                // await UniTask.Yield(ct);

                float time = GetTime();
                TrySpawn(time);

                await UniTask.Yield(ct);
            }
        }


        protected float GetTime() => Time.time;


        protected virtual List<Transform> GetSpawnPoints()
        {
            SpawnPoints = GetComponentsInChildren<Transform>().Where(it => it != transform).ToList();
            return SpawnPoints;
        }


        private bool TrySpawn(float time, WorldObjectId objId = null)
        {
            if (!CanSpawn(time))
            {
                return false;
            }

            WorldObjectId id = objId;
            if (id == null)
            {
                id = CalcNextObjectId();
            }

            RunObjectAsync(id).Forget();

            return true;
        }


        private async UniTask RunObjectAsync(WorldObjectId id)
        {
            Transform spawnPoint = GetSpawnPoint();
            WorldObjectFlow objFlow = CreateObject(id, spawnPoint, false);

            Transform objTransform = objFlow.transform;
            objTransform.SetParent(_spawnerFlow.ObjectsRoot);

            objTransform.position = spawnPoint.position;
            objTransform.rotation = spawnPoint.rotation;

            objFlow.gameObject.SetActive(true);
            _world.AddObject(objFlow);

            _spawnerFlow.Objects.Add(objFlow);
            OnSpawn();

            await objFlow.RunAsync();
            await OnRunObjectAsync(objFlow);
            objFlow.Clear();

            _spawnerFlow.Objects.Remove(objFlow);
            _world.RemoveObject(objFlow);
            DestroyObject(objFlow);

            OnDespawn();
        }


        protected virtual UniTask OnRunObjectAsync(WorldObjectFlow flow) => UniTask.CompletedTask;


        public virtual Transform GetSpawnPoint()
        {
            var points = GetSpawnPoints();
            int index = Random.Range(0, points.Count);
            return points[index];
        }


        protected virtual bool CanSpawn(float time) => true;
        protected virtual WorldObjectId CalcNextObjectId() => _spawnerFlow.ObjectIds.FirstOrDefault();

        protected virtual void OnSpawn() { }
        protected virtual void OnDespawn() { }


        protected abstract WorldObjectFlow CreateObject(WorldObjectId id,
            Transform spawnPoint,
            bool activateOnPop = true);


        protected abstract void DestroyObject(WorldObjectFlow flow);
    }
}
