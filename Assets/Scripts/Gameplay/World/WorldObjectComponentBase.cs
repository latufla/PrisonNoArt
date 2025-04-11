using UnityEngine;
using Zenject;


namespace Honeylab.Gameplay.World
{
    public abstract class WorldObjectComponentBase : MonoBehaviour
    {
        private WorldObjectFlow _worldObjectFlow;


        public T GetFlow<T>() where T : WorldObjectFlow => (T)_worldObjectFlow;
        public WorldObjectFlow GetFlow() => _worldObjectFlow;


        public void Init(WorldObjectFlow flow)
        {
            _worldObjectFlow = flow;

            OnInit();
        }


        public void Run()
        {
            OnRun();
        }


        public void Stop()
        {
            OnStop();
        }


        public void Clear()
        {
            OnClear();

            _worldObjectFlow = null;
        }


        protected virtual void OnInit() { }
        protected virtual void OnRun() { }
        protected virtual void OnStop() { }
        protected virtual void OnClear() { }

        protected bool IsInited() => _worldObjectFlow != null;
    }
}
