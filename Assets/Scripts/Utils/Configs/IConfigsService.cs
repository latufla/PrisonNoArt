namespace Honeylab.Utils.Configs
{
    public interface IConfigsService
    {
        bool TryGet<T>(string configId, out T config);
    }
}
