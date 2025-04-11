using UniRx;


namespace Honeylab.Utils.Extensions
{
    public static class AsyncSubjectExtension
    {
        public static void OnNextAndComplete<T>(this AsyncSubject<T> asyncSubject, T value)
        {
            asyncSubject.OnNext(value);
            asyncSubject.OnCompleted();
        }
    }
}
