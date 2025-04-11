using Cysharp.Threading.Tasks;
using Honeylab.Analytics;
using Honeylab.Gameplay.Cameras;
using Honeylab.Gameplay.Player;
using Honeylab.Gameplay.World;
using Honeylab.Sounds;
using Honeylab.Sounds.Data;
using Honeylab.Utils.Analytics;
using Honeylab.Utils.Extensions;
using Honeylab.Utils.Vfx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UniRx;
using UnityEngine;


namespace Honeylab.Gameplay
{
    public class PlayerDie : Die
    {
        [SerializeField] private float _duration;
        [SerializeField] private VfxId _deathVfxId;
        [SerializeField] private SoundId _deathSoundId;

        private VfxService _vfx;
        private SoundService _sounds;
        private PlayerSpawner _playerSpawner;
        private WorldObjectFlow _flow;
        private PlayerView _view;
        private PlayerInputService _playerInput;
        private CameraProvider _cameras;
        private PlayerInteraction _interaction;
        private IAnalyticsService _analyticsService;
        private readonly ISubject<Unit> _deathSubject = new Subject<Unit>();
        private readonly ISubject<Unit> _deathAnimEndSubject = new Subject<Unit>();
        private readonly ISubject<Unit> _onRespawn = new Subject<Unit>();

        public IObservable<Unit> OnDeathPlayer => _deathSubject.AsObservable();
        public IObservable<Unit> OnDeathAfterAnimEndPlayer => _deathAnimEndSubject.AsObservable();
        public IObservable<Unit> OnRespawn => _onRespawn.AsObservable();


        protected override void OnInit()
        {
            base.OnInit();
            _flow = GetFlow();
            _vfx = _flow.Resolve<VfxService>();
            _sounds = _flow.Resolve<SoundService>();
            _view = _flow.Get<PlayerView>();
            _playerInput = _flow.Resolve<PlayerInputService>();
            var world = _flow.Resolve<WorldObjectsService>();
            var spawnerFlow = world.GetObjects<PlayerSpawnerFlow>().First();
            _playerSpawner = spawnerFlow.Get<PlayerSpawner>();
            _cameras = _flow.Resolve<CameraProvider>();
            _interaction = _flow.Get<PlayerInteraction>();
            _analyticsService = _flow.Resolve<IAnalyticsService>();
        }


        protected override async UniTask OnDeath(CancellationToken ct)
        {
            _deathSubject.OnNext();

            SendAnalytics();

            using (_playerInput.BlockInput())
            {
                await UniTask.Yield(PlayerLoopTiming.FixedUpdate, ct);

                PlayDeathSound();

                Transform deathPoint = await PlayDeathAnimation(ct);
                PlayDeathVfx(deathPoint);
                PlayDeathVfxSound();

                _view.Animations.PlayIdle();

                await UniTask.Yield(ct);

                _view.gameObject.SetActive(false);
            }

            _deathAnimEndSubject.OnNext();
        }


        public async UniTask RespawnPlayer()
        {
            _interaction.ExitInteract();

            _flow.Clear();
            _onRespawn.OnNext();

            await UniTask.Yield();
            _cameras.PlayerCamera.Follow = _playerSpawner.GetSpawnPoint();
            _view.gameObject.SetActive(true);
        }


        private void SendAnalytics()
        {
            var msg = new Dictionary<string, object>
            {
                [AnalyticsParameters.Name] = "Death",
                [AnalyticsParameters.Amount] = 0
            };
            _analyticsService.ReportEvent(AnalyticsKeys.PlayerDeath, msg);
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
            var deathPoint = _view.Animations.PlayDie();
            await UniTask.Delay(TimeSpan.FromSeconds(_duration), cancellationToken: ct);
            return deathPoint;
        }


        private void PlayDeathVfxSound()
        {
            //_sounds.RequestSoundPlay(_sounds.Playlist.Death);
        }
    }
}
