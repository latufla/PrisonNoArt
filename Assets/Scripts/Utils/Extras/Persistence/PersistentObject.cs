using Newtonsoft.Json;
using System;
using System.Collections.Generic;


namespace Honeylab.Utils.Persistence
{
    [Serializable]
    public class PersistentObject
    {
        [JsonProperty("I")] internal PersistenceId _id { get; private set; }


        [JsonProperty("C")]
        internal List<ComponentBinding> _bindings { get; set; } = new List<ComponentBinding>();


        public PersistentObject() { }


        public PersistentObject(PersistenceId id)
        {
            _id = id;
        }


        public T FirstOrDefault<T>() where T : PersistentComponent
        {
            ComponentBinding first = null;
            foreach (ComponentBinding binding in _bindings)
            {
                if (binding.C is T)
                {
                    first = binding;
                    break;
                }
            }

            return (T)first?.C;
        }


        public T First<T>() where T : PersistentComponent
        {
            T first = FirstOrDefault<T>();
            if (first == default)
            {
                throw new InvalidOperationException();
            }

            return first;
        }


        public bool Has<T>() where T : PersistentComponent
        {
            foreach (ComponentBinding binding in _bindings)
            {
                if (binding.C is T)
                {
                    return true;
                }
            }

            return false;
        }


        public bool TryGetFirst<T>(out T first) where T : PersistentComponent
        {
            first = FirstOrDefault<T>();
            return first != default;
        }


        public T Add<T>() where T : PersistentComponent, new()
        {
            T component = new T();
            _bindings.Add(new ComponentBinding
            {
                C = component
            });
            return component;
        }


        public T GetOrAdd<T>() where T : PersistentComponent, new()
        {
            T component = FirstOrDefault<T>();
            component ??= Add<T>();
            return component;
        }


        public bool Any<T>(Predicate<T> predicate) where T : PersistentComponent =>
            TryGetFirst(out T firstOfT) && predicate(firstOfT);


        public void RemoveAllOf<T>() where T : PersistentComponent
        {
            for (int i = _bindings.Count - 1; i >= 0; i--)
            {
                if (_bindings[i].C is T)
                {
                    _bindings.RemoveAt(i);
                }
            }
        }
    }
}
