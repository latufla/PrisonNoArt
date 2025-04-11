using UniRx;


namespace Honeylab.Utils.Extensions
{
    public static class SubjectExtension
    {
        public static void OnNext(this ISubject<Unit> subject) => subject.OnNext(Unit.Default);
    }
}
