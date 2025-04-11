namespace Honeylab.Utils.Pool
{
    public interface IPrewarmPool
    {
        public int PrewarmAmount { get; }

        void EnsurePooledObjectCount(int count);
    }
}
