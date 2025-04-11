namespace Honeylab.Gameplay.Creatures
{
    public class FloatRange
    {
        public float Min { get; set; }
        public float Max { get; set; }
    }

    public class EnemySpawnerConfig
    {
        public FloatRange Delay { get; set; }
        public FloatRange Interval { get; set; }
        public int MaxCount { get; set; }
        public int MaxInLifetimeCount { get; set; }
    }
}
