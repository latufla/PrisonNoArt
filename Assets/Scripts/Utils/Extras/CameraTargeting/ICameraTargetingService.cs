using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;


namespace Honeylab.Utils.CameraTargeting
{
    public interface ICameraTargetingService
    {
        ICameraTargetingHandle Enqueue(Transform transform);
        ICameraTargetingHandle Enqueue(Transform transform, CameraTargetingOverrides overrides);
        bool IsRunningTarget(Transform transform);
        UniTask WaitForRunningTargetFocusAsync(CancellationToken ct);
    }
}
