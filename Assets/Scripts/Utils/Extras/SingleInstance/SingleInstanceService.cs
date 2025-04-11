using System;
using System.Collections.Generic;
using UniRx;


namespace Honeylab.Utils.SingleInstance
{
    public class SingleInstanceService<T> : IDisposable
    {
        private readonly List<T> _instances;
        private readonly CompositeDisposable _disposable;
        private readonly ReactiveProperty<T> _activeInstanceProp;


        public SingleInstanceService()
        {
            _instances = new List<T>();
            _disposable = new CompositeDisposable();
            _activeInstanceProp = new ReactiveProperty<T>(default).AddTo(_disposable);
        }


        public void AddInstance(T instance)
        {
            _instances.Add(instance);
            _activeInstanceProp.Value = _instances[_instances.Count - 1];
        }


        public void RemoveInstance(T instance)
        {
            _instances.Remove(instance);
            _activeInstanceProp.Value = _instances.Count > 0 ? _instances[_instances.Count - 1] : default;
        }


        public IObservable<T> ActiveInstanceAsObservable() => _activeInstanceProp.AsObservable();


        public void Dispose() => _disposable.Dispose();
    }
}
