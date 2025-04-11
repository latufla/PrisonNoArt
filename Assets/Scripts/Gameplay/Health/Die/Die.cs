using Cysharp.Threading.Tasks;
using Honeylab.Gameplay.World;
using Honeylab.Utils.Extensions;
using System.Threading;
using UnityEngine;


namespace Honeylab.Gameplay
{
    public abstract class Die : WorldObjectComponentBase
    {
        [SerializeField] private Health _health;

        private CancellationTokenSource _run;


        public bool IsDying { get; private set; }
        public bool IsDead { get; private set; }


        protected override void OnRun()
        {
            _run = new CancellationTokenSource();

            IsDying = false;
            IsDead = false;

            CancellationToken ct = _run.Token;
            RunDieAsync(ct).Forget();
        }


        protected override void OnStop()
        {
            _run?.CancelThenDispose();
            _run = null;
        }


        protected override void OnClear()
        {
            IsDying = false;
            IsDead = false;
        }


        private async UniTask RunDieAsync(CancellationToken ct)
        {
            IsDead = _health.HealthProp.Value <= 0.0f;

            while (true)
            {
                if (IsDead)
                {
                    await UniTask.Yield(ct);
                    continue;
                }

                await UniTask.WaitUntil(() => _health.HealthProp.Value <= 0.0f, cancellationToken: ct);

                IsDying = true;

                await OnDeath(ct);

                IsDead = true;
            }
        }


        protected abstract UniTask OnDeath(CancellationToken ct);
    }
}
