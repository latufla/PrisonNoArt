using Cysharp.Threading.Tasks;
using System.Threading;


namespace Honeylab.Gameplay.Buildings.View
{
    public class UnlockBuildingDie : Die
    {
        private UnlockBuildingFlow _flow;


        protected override void OnInit()
        {
            _flow = GetFlow<UnlockBuildingFlow>();
        }

        protected override UniTask OnDeath(CancellationToken ct)
        {
            _flow.State.Value = UnlockBuildingStates.Unlocked;
            return UniTask.CompletedTask;
        }
    }
}
