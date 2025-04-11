using Newtonsoft.Json;


namespace Honeylab.Utils.Persistence
{
    public interface IShouldSerialize
    {
        [JsonIgnore] bool ShouldSerialize { get; }
    }
}
