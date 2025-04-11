using System;
using System.Collections.Generic;
using UniRx;


namespace Honeylab.Utils.Extensions
{
    public static class ObservablesEnumerableExtension
    {
        public static IObservable<int> LatestSumNonAlloc(this IEnumerable<IObservable<int>> observables) => observables
            .CombineLatest()
            .Select(xs => xs.SumNonAlloc());


        public static IObservable<float> LatestSumNonAlloc(this IEnumerable<IObservable<float>> observables) =>
            observables
                .CombineLatest()
                .Select(xs => xs.SumNonAlloc());
    }
}
