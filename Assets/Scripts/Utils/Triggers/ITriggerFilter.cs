namespace Honeylab.Utils.Triggers
{
    public interface ITriggerFilter<in T>
    {
        bool ShouldPassObject(T obj);
    }
}
