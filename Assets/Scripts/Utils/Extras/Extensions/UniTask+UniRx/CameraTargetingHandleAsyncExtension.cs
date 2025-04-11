using Cysharp.Threading.Tasks;
using Honeylab.Utils.CameraTargeting;
using System.Threading;
using UniRx;


namespace Honeylab.Utils.Extensions
{
    public static class CameraTargetingHandleAsyncExtension
    {
        public static UniTask WaitForFocusAsync(this ICameraTargetingHandle handle, CancellationToken ct)
        {
            if (ct.IsCancellationRequested)
            {
                return UniTask.FromCanceled(ct);
            }

            if (handle.IsFocused)
            {
                return UniTask.CompletedTask;
            }

            return Observable.FromEvent<ICameraTargetingHandle>(h => handle.Focused += h,
                    h => handle.Focused -= h)
                .ToUniTask(true, ct);
        }
    }
}
