using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using System;
using System.Threading;


namespace Honeylab.Utils.Extensions
{
    public static class ObservableAsyncExtension
    {
        public static UniTask<bool> AnyAsync<T>(this IObservable<T> observable,
            Func<T, bool> predicate,
            CancellationToken ct) => observable
            .ToUniTaskAsyncEnumerable()
            .AnyAsync(predicate, ct);


        public static UniTask<bool> AnyAsync<T>(this IObservable<T> observable, CancellationToken ct) => observable
            .ToUniTaskAsyncEnumerable()
            .AnyAsync(ct);
    }
}
