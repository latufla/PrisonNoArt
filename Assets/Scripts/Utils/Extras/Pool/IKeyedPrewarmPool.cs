using System.Collections.Generic;


namespace Honeylab.Utils.Pool
{
    public interface IKeyedPrewarmPool<T>
    {
        public int PrewarmAmount { get; }

        public abstract IGameObjectPool Get(T key);
        public abstract IEnumerable<T> GetKeys();
    }
}
