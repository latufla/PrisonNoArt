using Cinemachine;
using Cysharp.Threading.Tasks;
using System.Threading;


namespace Honeylab.Utils.Extensions
{
    public static class CinemachineBrainExtension
    {
        public static async UniTask WaitForBlendCompletionAsync(this CinemachineBrain brain,
            CancellationToken ct,
            bool performInitialManualUpdate = true)
        {
            if (performInitialManualUpdate)
            {
                brain.ManualUpdate();
            }

            while (brain.IsBlending)
            {
                await UniTask.Yield(ct);
            }
        }
    }
}
