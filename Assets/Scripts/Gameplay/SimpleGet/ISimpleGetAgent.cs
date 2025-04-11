using Honeylab.Gameplay.SpeedUp;
using Honeylab.Gameplay.World;

namespace Honeylab.Gameplay.SimpleGet
{
    public interface ISimpleGetAgent
    {
        WorldObjectId Id { get; }
        string ConfigId { get; }
        T Resolve<T>();
    }
}
