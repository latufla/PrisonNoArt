using UnityEngine;


namespace Honeylab.Utils.ProbabilityCalculation
{
    public class UnityRandomImpl : IRandom
    {
        public float NextFloat(float maxValue) => Random.Range(0.0f, maxValue);
    }
}
