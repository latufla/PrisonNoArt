using Cysharp.Threading.Tasks;
using Honeylab.Gameplay.Creatures;
using Honeylab.Gameplay.World;
using Honeylab.Sounds;
using Honeylab.Sounds.Data;
using Honeylab.Utils.Vfx;
using System;
using System.Threading;
using UnityEngine;


namespace Honeylab.Gameplay
{
    public class EnemySimpleDie : Die
    {
        [SerializeField] private float _duration;
        [SerializeField] private VfxId _deathVfxId;
        [SerializeField] private SoundId _deathSoundId;

        private EnemyView _view;
        private VfxService _vfx;
        private SoundService _sounds;
        private WorldObjectFlow _flow;
        private WorldObjectsService _world;


        protected override void OnInit()
        {
            base.OnInit();

            _flow = GetFlow();

            _view = _flow.Get<EnemyView>();
            _vfx = _flow.Resolve<VfxService>();
            _sounds = _flow.Resolve<SoundService>();
            _world = _flow.Resolve<WorldObjectsService>();

            _view.SkinView.Collider.enabled = true;
        }


        protected override async UniTask OnDeath(CancellationToken ct)
        {
            SendDieEvent();
            _view.SkinView.Collider.enabled = false;
            
            PlayDeathSound();

            Transform deathPoint = await PlayDeathAnimation(ct);
            PlayDeathVfx(deathPoint);
        }


        private void PlayDeathVfx(Transform deathPoint)
        {
            _vfx.PlayOnceAsync(_deathVfxId, deathPoint.position, Quaternion.identity).Forget();
        }


        private void PlayDeathSound()
        {
            _sounds.RequestSoundPlay(_deathSoundId);
        }


        private async UniTask<Transform> PlayDeathAnimation(CancellationToken ct)
        {
            await UniTask.Yield(ct);

            Transform deathPoint = _view.Animations.PlayDie();

            await UniTask.Delay(TimeSpan.FromSeconds(_duration), cancellationToken: ct);
            return deathPoint;
        }


        private void SendDieEvent()
        {
            _world.OnEnemyDiedEvent(_flow.Id);
        }
    }
}
