using Cysharp.Threading.Tasks;
using Honeylab.Gameplay.World;
using System.Threading;

namespace Honeylab.Gameplay
{
    public class WorldObjectDie : Die
    {
        private WorldObjectFlow _flow;
        private WorldObjectsService _world;
        protected override void OnInit()
        {
            base.OnInit();

            _flow = GetFlow();
            _world = _flow.Resolve<WorldObjectsService>();
        }

        protected override UniTask OnDeath(CancellationToken ct)
        {
            SendDieEvent();

            return UniTask.CompletedTask;
        }

        private void SendDieEvent()
        {
            _world.OnObjectDiedEvent(_flow.Id);
        }
    }
}
