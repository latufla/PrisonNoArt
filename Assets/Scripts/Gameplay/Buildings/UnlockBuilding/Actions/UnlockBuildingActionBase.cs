using Cysharp.Threading.Tasks;
using Honeylab.Gameplay.Buildings.View;
using Honeylab.Gameplay.World;
using System.Threading;


namespace Honeylab.Gameplay.Buildings
{
    public class UnlockBuildingActionBase : WorldObjectComponentBase
    {
        protected UnlockBuildingFlow Flow;
        protected UnlockBuildingView View;


        protected override void OnInit()
        {
            Flow = GetFlow<UnlockBuildingFlow>();
            View = Flow.Get<UnlockBuildingView>();
        }


        protected bool IsUnlocked() => Flow.State.Value == UnlockBuildingStates.Unlocked;


        public UniTask UnlockAsync(CancellationToken ct) => OnUnlockAsync(ct);


        protected virtual UniTask OnUnlockAsync(CancellationToken ct) => UniTask.CompletedTask;
    }
}
