using System;


namespace Honeylab.Utils.ProbabilityCalculation
{
    public class FrameworkRandomImpl : IRandom
    {
        private readonly Random _random;


        public FrameworkRandomImpl(Random random)
        {
            _random = random;
        }


        public float NextFloat(float maxValue) => (float)(_random.NextDouble() * maxValue);
    }
}
