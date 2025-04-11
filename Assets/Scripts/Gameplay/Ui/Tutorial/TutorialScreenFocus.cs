using Cysharp.Threading.Tasks;
using DG.Tweening;
using Honeylab.Gameplay.Buildings;
using Honeylab.Gameplay.Buildings.View;
using Honeylab.Gameplay.Player;
using Honeylab.Gameplay.Weapons;
using Honeylab.Gameplay.World;
using Honeylab.Utils.CameraTargeting;
using Honeylab.Utils.Extensions;
using System;
using System.Threading;
using UniRx;
using UnityEngine;


namespace Honeylab.Gameplay.Tutorial
{
    public class TutorialScreenFocus
    {
        private readonly TutorialScreen _screen;
        private readonly PlayerInputService _playerInputService;
        private readonly ICameraTargetingService _cameraTargetingService;
        private Transform _target;
        private bool _isFocused = false;

        private CompositeDisposable _disposable;
        private CancellationTokenSource _cts = new CancellationTokenSource();

        private IDisposable _clickHerePanelInputBlock;
        private static readonly int Focus = Animator.StringToHash("Focus");
        private static readonly int Idle = Animator.StringToHash("Idle");

        private readonly Subject<Transform> _onStartFocusSubjects = new Subject<Transform>();
        private readonly Subject<Transform> _onEndFocusSubjects = new Subject<Transform>();

        public IObservable<Transform> OnStartFocusSubjectsAsObservable() => _onStartFocusSubjects.AsObservable();
        public IObservable<Transform> OnEndFocusSubjectsAsObservable() => _onEndFocusSubjects.AsObservable();

        public TutorialScreenFocus(TutorialScreen screen,
            PlayerInputService playerInputService,
            ICameraTargetingService cameraTargetingService)
        {
            _screen = screen;
            _playerInputService = playerInputService;
            _cameraTargetingService = cameraTargetingService;
        }


        public void Run()
        {
            _disposable = new CompositeDisposable();

            CancellationToken ct = _cts.Token;
            IDisposable onFocus = _screen.OnFocusButtonClickAsObservable()
                .Subscribe(_ =>
                {
                    SetClickHerePanelActive(false);
                    FocusTargetAsync(_target, ct).Forget();
                });
            _disposable.Add(onFocus);
        }


        public void SetClickHerePanelActive(bool isActive)
        {
            _screen.ClickHerePanel.gameObject.SetActive(isActive);
            _screen.FocusImageActive(isActive);
            if (isActive)
            {
                _clickHerePanelInputBlock = _playerInputService.BlockInput();
                _screen.ClickHereAnimator.SetTrigger(Focus);
            }
            else if(_clickHerePanelInputBlock != null)
            {
                _clickHerePanelInputBlock?.Dispose();
                _clickHerePanelInputBlock = null;
                _screen.ClickHereAnimator.SetTrigger(Idle);
            }
        }


        public void FocusTarget(Transform target)
        {
            CancellationToken ct = _cts.Token;
            FocusTargetAsync(target, ct).Forget();
        }


        public async UniTask FocusTargetAsync(Transform target, CancellationToken ct, float duration = 1.0f)
        {
            if (_isFocused || target == null)
            {
                return;
            }

            _isFocused = true;

            _onStartFocusSubjects.OnNext(target);

            _screen.FocusButtonActive(false);
            _screen.RequiredItems.RequiredItemsButtonActive(false);
            using (_playerInputService.BlockInput())
            {
                ICameraTargetingHandle handler =
                    _cameraTargetingService.Enqueue(target);
                await handler.WaitForFocusAsync(ct);
                bool isColorChanged = await ChangeColorOnFocus(target, ct);

                if (!isColorChanged)
                {
                    await UniTask.Delay(TimeSpan.FromSeconds(duration), cancellationToken: ct);
                }

                handler.Finish();
            }

            _isFocused = false;
            _screen.FocusButtonActive(target != null);
            _screen.RequiredItems.RequiredItemsButtonActive(true);

            _onEndFocusSubjects.OnNext(target);
        }


        private async UniTask<bool> ChangeColorOnFocus(Transform target, CancellationToken ct)
        {
            WorldObjectComponentVisual viewBase = null;

            if (target.TryGetComponent(out WeaponAttackTarget tr))
            {
                viewBase = tr.View;
            }
            else if (target.TryGetComponent(out WorldObjectFlow flow))
            {
                if (flow.TryGet(out UnlockBuildingViewBase unlockView))
                {
                    viewBase = unlockView;
                }
                else if (flow.TryGet(out WeaponAttackTargetView weaponView))
                {
                    viewBase = weaponView;
                }
                else if (flow.TryGet(out CraftBuildingViewBase craftView))
                {
                    viewBase = craftView;
                }
                else if (flow.TryGet(out UpgradeBuildingView upgradeView))
                {
                    viewBase = upgradeView;
                }

            }

            if (viewBase == null)
            {
                return false;
            }

            float colorInDuration = 0.3f;
            float colorOutDuration = 0.3f;
            bool isColorChanged = viewBase.ChangeColorState(colorInDuration, colorOutDuration, Ease.InSine, ct);
            if (!isColorChanged)
            {
                return false;
            }

            await UniTask.Delay(TimeSpan.FromSeconds(colorInDuration + colorOutDuration + Time.deltaTime * 2), cancellationToken: ct);
            isColorChanged = viewBase.ChangeColorState(colorInDuration, colorOutDuration, Ease.InSine, ct);
            await UniTask.Delay(TimeSpan.FromSeconds(colorInDuration + colorOutDuration), cancellationToken: ct);
            return isColorChanged;
        }


        public void SetTarget(Transform target)
        {
            _target = target;
            bool active = _target != null;
            _screen.FocusButtonActive(active);
        }


        public void RemoveTarget()
        {
            _target = null;
            _screen.FocusButtonActive(false);
        }


        public void Stop()
        {
            _cts?.CancelThenDispose();
            _cts = null;

            _disposable?.Dispose();
            _disposable = null;
        }
    }
}
