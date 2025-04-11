using Cysharp.Threading.Tasks;
using Honeylab.Gameplay.Ui;
using Honeylab.Gameplay.World;
using Honeylab.Sounds;
using Honeylab.Sounds.Data;
using Honeylab.Utils;
using Honeylab.Utils.Vfx;
using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;


namespace Honeylab.Gameplay.Player
{
    public class PlayerView : WorldObjectComponentBase
    {
        [SerializeField] private PlayerAnimations _animations;
        [SerializeField] private Transform[] _billboardRoots;
        [SerializeField] private VfxId _stepVfxId;
        [SerializeField] private SoundId _stepSoundId;

        private VfxService _vfxs;
        private BillboardPresenterFactory _billboardsFactory;
        private SoundService _sounds;
        private PostProcessingEffectsService _postProcessingEffects;

        private CompositeDisposable _disposable;
        private readonly List<BillboardPresenter> _billboards = new();


        protected override void OnInit()
        {
            WorldObjectFlow flow = GetFlow();
            _vfxs = flow.Resolve<VfxService>();
            _billboardsFactory = flow.Resolve<BillboardPresenterFactory>();
            _sounds = flow.Resolve<SoundService>();
            _postProcessingEffects = flow.Resolve<PostProcessingEffectsService>();

            _disposable = new CompositeDisposable();
            IDisposable stepVfx = _animations.OnStepAsObservable()
                .Subscribe(stepPosition =>
                {
                    _vfxs.PlayOnceAsync(_stepVfxId, stepPosition, Quaternion.identity).Forget();
                });
            _disposable.Add(stepVfx);

            var health = flow.Get<Health>();
            IDisposable takeDamage = health.HealthProp
                .Pairwise()
                .Where(hp => hp.Previous > hp.Current)
                .Subscribe(_ => { _postProcessingEffects.PlayDamage(); });
            _disposable.Add(takeDamage);

            foreach (Transform root in _billboardRoots)
            {
                BillboardPresenter billboard = _billboardsFactory.Create(root).AddTo(this);
                billboard.Run();

                _billboards.Add(billboard);
            }
        }


        protected override void OnClear()
        {
            _disposable?.Clear();
            _disposable?.Dispose();

            foreach (BillboardPresenter presenter in _billboards)
            {
                presenter.Dispose();
            }

            _billboards.Clear();
        }


        public PlayerAnimations Animations => _animations;
        public Transform Transform => transform;


        private void PlayStepSound()
        {
            _sounds.RequestSoundPlay(_stepSoundId);
        }


        private void StopStepSound()
        {
            _sounds.RequestSoundStop(_stepSoundId);
        }
    }
}
