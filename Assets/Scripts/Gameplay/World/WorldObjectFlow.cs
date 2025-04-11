using Cysharp.Threading.Tasks;
using Honeylab.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using UnityEngine;
using Zenject;


namespace Honeylab.Gameplay.World
{
    public class WorldObjectFlow : MonoBehaviour
    {
        [SerializeField] private WorldObjectIdProvider _id;
        [SerializeField] private int _order;

        private DiContainer _di;

        private List<WorldObjectComponentBase> _components;
        private CancellationTokenSource _run;


        public WorldObjectId Id => _id.Id;
        public int Order => _order;

        public bool IsInited => _di != null;
        public bool IsRunning => _run != null;


        public void Init(DiContainer di)
        {
            if (IsInited)
            {
                return;
            }

            _di = di;

            OnInit();

            //_components = GetComponentsInChildren<WorldObjectComponentBase>().Where(it => it.transform.GetComponentInParent<WorldObjectFlow>(true).Id.Equals(Id)).ToList();

            _components = GetComponentsInChildren<WorldObjectComponentBase>().ToList();
            var oldCom = new List<WorldObjectComponentBase>(_components);
            foreach (var component in oldCom)
            {
                var par = component.transform.GetComponentInParent<WorldObjectFlow>(true);
                if (par == null)
                {
                    throw new Exception(name + " " + component.name);
                }

                if (par.Id == null)
                {
                    throw new Exception(name + " " + par.name);
                }

                if (par.Id.Equals(Id))
                {
                    continue;
                }

                Debug.LogWarning(name + " " + par.name);
                _components.Remove(component);
            }

            _components.ForEach(it => it.Init(this));
        }


        protected void SetId(WorldObjectId newId)
        {
            _id.SetId(newId);
        }


        public async UniTask RunAsync()
        {
            if (_run != null)
            {
                return;
            }

            _components.ForEach(it => it.Run());

            _run = new CancellationTokenSource();
            CancellationToken ct = _run.Token;
            await OnRunAsync(ct);
        }


        public void Stop()
        {
            OnStop();
            _components.ForEach(it => it.Stop());

            _run?.CancelThenDispose();
            _run = null;
        }


        public void Clear()
        {
            Stop();

            OnClear();

            _components.ForEach(it => it.Clear());
            _components.Clear();

            _di = null;
        }


        public T Get<T>() where T : WorldObjectComponentBase => _components.OfType<T>().FirstOrDefault();


        public bool TryGet<T>(out T componentBase) where T : WorldObjectComponentBase
        {
            componentBase = Get<T>();
            return componentBase != null;
        }


        public T Resolve<T>() => _di.Resolve<T>();


        protected virtual void OnInit() { }

        protected virtual UniTask OnRunAsync(CancellationToken ct) => UniTask.CompletedTask;

        protected virtual void OnStop() { }
        protected virtual void OnClear() { }


        public Guid CalcGuidFromName()
        {
            Guid guid;
            using (MD5 md5 = MD5.Create())
            {
                byte[] hash = md5.ComputeHash(Encoding.UTF8.GetBytes(gameObject.name));
                guid = new Guid(hash);
            }

            return guid;
        }
    }
}
