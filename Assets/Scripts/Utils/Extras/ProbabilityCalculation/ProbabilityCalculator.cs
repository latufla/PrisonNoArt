using System;
using System.Collections.Generic;
using UnityEngine;


namespace Honeylab.Utils.ProbabilityCalculation
{
    public static class ProbabilityCalculator
    {
        private static readonly IRandom DefaultRandom = new UnityRandomImpl();


        public static int GetChoiceIndexWithWeights(IRandom random = null, params float[] weights)
        {
            EnsureNonNullRandom(ref random);
            return GetChoiceIndexWithWeights(new FloatWeightProvider(weights), random);
        }


        public static int GetChoiceIndexWithWeights(IReadOnlyList<int> weights, IRandom random = null)
        {
            EnsureNonNullRandom(ref random);
            return GetChoiceIndexWithWeights(new IntWeightProvider(weights), random);
        }


        public static int GetChoiceIndexWithWeights<T>(IReadOnlyList<T> weights, IRandom random = null) where T : IWeight
        {
            EnsureNonNullRandom(ref random);
            return GetChoiceIndexWithWeights(new WeightListWeightProvider<T>(weights), random);
        }


        public static int GetChoiceIndexWithWeights<T>(IReadOnlyList<T> items,
            Func<T, float> weightsFunc,
            IRandom random = null)
        {
            EnsureNonNullRandom(ref random);
            return GetChoiceIndexWithWeights(new FuncWeightProvider<T>(items, weightsFunc), random);
        }


        public static int GetChoiceIndexWithWeights(IWeightProvider weights, IRandom random)
        {
            if (weights == null)
            {
                throw new ArgumentNullException(nameof(weights));
            }

            if (weights.Count() == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(weights), "Length is zero");
            }

            if (weights.Count() == 1)
            {
                return 0;
            }

            float totalLineLength = 0.0f;
            for (int i = 0; i < weights.Count(); ++i)
            {
                totalLineLength += weights.GetWeightAtIdx(i);
            }

            float linePoint = random.NextFloat(totalLineLength);
            float accumulatedProb = 0.0f;
            int choiceIndex = -1;

            for (int i = 0; i < weights.Count(); ++i)
            {
                float previousAccumulation = accumulatedProb;
                accumulatedProb += weights.GetWeightAtIdx(i);

                if (linePoint <= accumulatedProb && linePoint > previousAccumulation)
                {
                    choiceIndex = i;
                    break;
                }
            }

            return choiceIndex;
        }


        private static void EnsureNonNullRandom(ref IRandom randomRef)
        {
            if (randomRef == null)
            {
                randomRef = DefaultRandom;
            }
        }
    }


    [Serializable]
    public class WeightedValue<T> : IWeight
    {
        public T value;
        [SerializeField] [Min(0)] private int _weight;


        public float GetWeight() => _weight;
    }
}
