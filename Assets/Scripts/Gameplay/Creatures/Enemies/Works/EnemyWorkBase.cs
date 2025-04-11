using Cysharp.Threading.Tasks;
using Honeylab.Gameplay.Weapons;
using Honeylab.Gameplay.World;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;


namespace Honeylab.Gameplay.Creatures
{
    public abstract class EnemyWorkBase : WeaponAgentBase
    {
        protected EnemyFlow Flow;
        protected EnemyView View;
        protected CreatureMotion Motion;
        protected Die Die;


        protected override void OnInit()
        {
            Flow = GetFlow<EnemyFlow>();
            View = Flow.Get<EnemyView>();
            Motion = Flow.Get<CreatureMotion>();
            Die = Flow.Get<Die>();
        }


        public virtual bool CanExecute() => true;

        protected virtual UniTask Execute(CancellationToken ct) => UniTask.CompletedTask;


        public async UniTask<bool> TryExecute(CancellationToken ct)
        {
            if (CanExecute())
            {
                await Execute(ct);
                return true;
            }

            return false;
        }


        protected async UniTask MoveToTargetAsync(Vector3 destination,
            List<EnemyFlow> enemies = null,
            float nearbyAgentDistance = 0)
        {
            if (Motion.IsDestinationReached(destination))
            {
                return;
            }

            View.Animations.PlayWalk();
            await Motion.MoveToTargetAsync(destination, enemies, nearbyAgentDistance);
            View.Animations.PlayIdle();
        }


        protected async UniTask MoveToTargetAsync(Vector3 destination,
            Vector3 endDirection,
            List<EnemyFlow> enemies = null,
            float nearbyAgentDistance = 0)
        {
            await MoveToTargetAsync(destination, enemies, nearbyAgentDistance);
            Motion.RotateTowards(endDirection);
        }


        protected void StopMoveToTarget(bool playIdle = true)
        {
            Motion.StopMoveToTarget();

            if (playIdle)
            {
                View.Animations.PlayIdle();
            }
        }


        protected void StopMoveToTarget(Vector3 endDirection, bool playIdle = true)
        {
            Motion.StopMoveToTarget();
            Motion.RotateTowards(endDirection);

            if (playIdle)
            {
                View.Animations.PlayIdle();
            }
        }
    }
}
