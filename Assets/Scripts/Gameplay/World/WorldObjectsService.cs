using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UniRx;
using Zenject;


namespace Honeylab.Gameplay.World
{
    public class WorldObjectsService : IDisposable
    {
        private readonly WorldObjectsArgs _args;
        private readonly DiContainer _di;

        private readonly List<WorldObjectFlow> _objects = new();
        private readonly List<WorldObjectFlow> _registeredObjects = new();

        private Subject<WorldObjectId> _onObjectDiedEvent = new Subject<WorldObjectId>();
        private Subject<WorldObjectId> _onEnemyDiedEvent = new Subject<WorldObjectId>();
        public IReadOnlyList<WorldObjectFlow> GetObjects() => _objects;
        public T FindFirstOrDefault<T>() where T : WorldObjectFlow => _objects.Find(it => it is T) as T;
        public IReadOnlyList<T> GetObjects<T>() where T : WorldObjectFlow => _objects.OfType<T>().ToList();

        [Description("Active and inactive objects")]
        public IReadOnlyList<WorldObjectFlow> GetRegisteredObjects() => _registeredObjects;

        [Description("Active and inactive objects")]
        public T FindRegisteredFirstOrDefault<T>() where T : WorldObjectFlow =>
            _registeredObjects.Find(it => it is T) as T;

        [Description("Active and inactive objects")]
        public IReadOnlyList<T> GetRegisteredObjects<T>() where T : WorldObjectFlow =>
            _registeredObjects.OfType<T>().ToList();
        public IObservable<WorldObjectId> OnObjectDiedAsObservable() => _onObjectDiedEvent.AsObservable();
        public IObservable<WorldObjectId> OnEnemyDiedAsObservable() => _onEnemyDiedEvent.AsObservable();

        public WorldObjectsService(WorldObjectsArgs args, DiContainer di)
        {
            _args = args;
            _di = di;
        }


        public void Init()
        {
            var activeFlows = _args.Root.GetComponentsInChildren<WorldObjectFlow>()
                .OrderBy(it => it.Order)
                .ToList();

            var flowsToAdd = activeFlows.Where(f => f.Order <= 0);
            foreach (WorldObjectFlow f in flowsToAdd)
            {
                AddObject(f);
            }

            flowsToAdd = activeFlows.Where(f => f.gameObject is { activeSelf: true, activeInHierarchy: true } && f.Order > 0);
            foreach (WorldObjectFlow f in flowsToAdd)
            {
                AddObject(f);
            }

            var flows = _args.Root.GetComponentsInChildren<WorldObjectFlow>(true);
            foreach (WorldObjectFlow f in flows)
            {
                RegisterObject(f);
            }
        }


        public void Run()
        {
            for (int i = 0; i < _objects.Count; i++)
            {
                RunObject(_objects[i]);
            }
        }


        public void Dispose()
        {
            var toRemoved = new List<WorldObjectFlow>(_objects); //TODO: Collection was modified when dispose
            toRemoved.ForEach(it => it.Clear());
            _objects.Clear();
        }


        public void AddObject(WorldObjectFlow flow)
        {
            if (_objects.Contains(flow))
            {
                return;
            }

            int index = _objects.FindIndex(it => flow.Order < it.Order);
            if (index == -1)
            {
                _objects.Add(flow);
            }
            else
            {
                _objects.Insert(index, flow);
            }

            flow.Init(_di);
        }


        public void RemoveObject(WorldObjectFlow flow)
        {
            if (!_objects.Contains(flow))
            {
                return;
            }

            _objects.Remove(flow);
            flow.Clear();
        }


        public void RunObject(WorldObjectId id)
        {
            var flow = _objects.First(it => it.Id.Equals(id));
            flow.RunAsync().Forget();
        }


        public void RunObject(WorldObjectFlow flow)
        {
            if (!_objects.Contains(flow))
            {
                throw new Exception("Object is not added " + flow.Id);
            }

            flow.RunAsync().Forget();
        }


        public void StopObject(WorldObjectId id)
        {
            var flow = _objects.First(it => it.Id.Equals(id));
            flow.Stop();
        }


        public WorldObjectFlow GetObject(WorldObjectId id)
        {
            return _objects.FirstOrDefault(it => it.Id.Equals(id));
        }


        public T GetObject<T>(WorldObjectId id) where T : WorldObjectFlow
        {
            return _objects.FirstOrDefault(it => it.Id.Equals(id)) as T;
        }


        public bool Contains(WorldObjectFlow obj) => _objects.Contains(obj);


        private void RegisterObject(WorldObjectFlow flow)
        {
            if (_registeredObjects.Contains(flow))
            {
                return;
            }

            _registeredObjects.Add(flow);
        }


        private void UnregisterObject(WorldObjectFlow flow)
        {
            if (!_registeredObjects.Contains(flow))
            {
                return;
            }

            _registeredObjects.Remove(flow);
        }


        public void OnObjectDiedEvent(WorldObjectId id)
        {
            _onObjectDiedEvent.OnNext(id);
        }


        public void OnEnemyDiedEvent(WorldObjectId id)
        {
            _onEnemyDiedEvent.OnNext(id);
        }
    }
}
