using Honeylab.Gameplay.World;
using UniRx;


namespace Honeylab.Gameplay.SpeedUp
{
    public interface ISpeedUpAgent
    {
        WorldObjectId Id { get; }
        ISpeedUpAgentConfig SpeedUpConfig { get; }
        float Duration { get; }
        IReactiveProperty<double> TimeLeft { get; }
        void SpeedUp(float time);

        T Resolve<T>();
    }
}
