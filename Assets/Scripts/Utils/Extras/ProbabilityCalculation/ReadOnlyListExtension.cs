using System.Collections.Generic;


namespace Honeylab.Utils.ProbabilityCalculation
{
    public static class ReadOnlyListExtension
    {
        public static T Yield<T>(this IReadOnlyList<T> list, IRandom random = null) where T : IWeight
        {
            int index = ProbabilityCalculator.GetChoiceIndexWithWeights(list, random);
            return list[index];
        }
    }
}
