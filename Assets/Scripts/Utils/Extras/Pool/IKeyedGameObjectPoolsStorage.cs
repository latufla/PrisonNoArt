namespace Honeylab.Utils.Pool
{
    public interface IKeyedGameObjectPoolsStorage<in T>
    {
        IGameObjectPool Get(T key);
    }
}
