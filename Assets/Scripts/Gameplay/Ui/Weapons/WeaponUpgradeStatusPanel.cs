using Cysharp.Threading.Tasks;
using Honeylab.Gameplay.Buildings;
using Honeylab.Gameplay.Player;
using Honeylab.Gameplay.Weapons;
using Honeylab.Utils.CameraTargeting;
using System;
using System.Threading;
using UniRx;
using UnityEngine;
using UnityEngine.UI;


namespace Honeylab.Gameplay.Ui.Upgrades
{
    public class WeaponUpgradeStatusPanel : UIStatusPanelBase
    {
        [SerializeField] private RectTransform _layout;
        private WeaponsData _weaponsData;
        private UpgradeBuildingFlow _upgradeBuildingFlow;
        private CompositeDisposable _disposable;

        public Sprite GetIconSprite() => Icon.sprite;
        public bool IsInited { get; private set; }


        public void Init(PlayerInputService playerInputService,
            ICameraTargetingService cameraTargetingService,
            WeaponsData weaponsData)
        {
            Init(playerInputService, cameraTargetingService);
            _weaponsData = weaponsData;

            Show();
            Cts ??= new CancellationTokenSource();
            CancellationToken ct = Cts.Token;
            RunAsync(ct).Forget();

            IsInited = true;
        }


        private async UniTask RunAsync(CancellationToken ct)
        {
            await UniTask.WaitUntil(() => _upgradeBuildingFlow != null, cancellationToken: ct);

            _disposable = new CompositeDisposable();
            IDisposable onStateChanged = _upgradeBuildingFlow.State.ValueProperty
                .Subscribe(state => { ChangeState(state, ct); });
            _disposable.Add(onStateChanged);

            IDisposable onFocused = OnFocusedTarget()
                .Subscribe(active => ActiveUpgradePopup(_upgradeBuildingFlow, active));
            _disposable.Add(onFocused);

            IDisposable onButtonClick = OnButtonClickAsObservable()
                .Subscribe(_ => { FocusTarget(_upgradeBuildingFlow.transform, ct).Forget(); });
            _disposable.Add(onButtonClick);
        }


        private void ChangeState(UpgradeBuildingStates state, CancellationToken ct)
        {
            switch (state)
            {
                case UpgradeBuildingStates.Idle:
                    IdleWork();
                    break;

                case UpgradeBuildingStates.Work:
                    WorkAsync(_upgradeBuildingFlow, ct).Forget();
                    break;

                case UpgradeBuildingStates.Done:
                    CompleteWork(_upgradeBuildingFlow, ct).Forget();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }


        private void ActiveUpgradePopup(UpgradeBuildingFlow flow, bool show)
        {
            UpgradeBuildingInteractable interactable = flow.Get<UpgradeBuildingInteractable>();
            if (interactable.HasAgent())
            {
                return;
            }

            if (show)
            {
                interactable.ShowUpgradePopup();
            }
            else
            {
                interactable.HideUpgradePopup();
            }
        }


        public void SetBuilding(UpgradeBuildingFlow flow)
        {
            _upgradeBuildingFlow ??= flow;
        }


        private async UniTask WorkAsync(UpgradeBuildingFlow flow, CancellationToken ct)
        {
            InProcess = true;

            await UniTask.WaitUntil(() => PlayerInputService != null && CameraTargetingService != null,
                cancellationToken: ct);

            if (Disposable != null)
            {
                return;
            }

            Show();

            await UniTask.WaitUntil(() =>
                {
                    Animator.SetInteger(ProcessState, 1);
                    return Animator.GetInteger(ProcessState) == 1;
                },
                cancellationToken: ct);

            UpdateInfo(flow);

            ProgressPanel.gameObject.SetActive(true);

            Disposable = new CompositeDisposable();
            IDisposable updateProgress = flow.TimeLeft.Subscribe(timeLeft =>
            {
                ProgressPanel.SetTime((float)timeLeft, flow.WeaponUpgradeConfig.Duration);
            });
            Disposable.Add(updateProgress);
        }


        private async UniTask CompleteWork(UpgradeBuildingFlow flow, CancellationToken ct)
        {
            InProcess = true;

            Disposable?.Dispose();
            Disposable = null;

            Show();

            await UniTask.WaitUntil(() =>
                {
                    Animator.SetInteger(ProcessState, 2);
                    return Animator.GetInteger(ProcessState) == 2;
                },
                cancellationToken: ct);


            UpdateInfo(flow);
        }


        private void UpdateInfo(UpgradeBuildingFlow flow)
        {
            int level = flow.WeaponUpgrade.GetLevel();
            WeaponData weaponData = _weaponsData.GetData(flow.WeaponId);
            WeaponLevelData weaponLevelData = weaponData.Levels[level - 1];
            SetInfo(level + 1, weaponLevelData.Sprite);
        }


        public void SetInfo(int level, Sprite sprite)
        {
            SetIcon(sprite);
            SetText(level);

            LayoutRebuilder.ForceRebuildLayoutImmediate(_layout);
        }


        protected override void OnClear()
        {
            _disposable?.Dispose();
            _disposable = null;
        }
    }
}
