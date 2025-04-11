using System.Threading;


namespace Honeylab.Utils.Extensions
{
    public static class CancellationTokenSourceExtension
    {
        public static void CancelThenDispose(this CancellationTokenSource cts)
        {
            cts.Cancel();
            cts.Dispose();
        }
    }
}
