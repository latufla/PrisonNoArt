using Cysharp.Threading.Tasks;
using Honeylab.Consumables.Configs;
using Honeylab.Gameplay.Buildings.Config;
using Honeylab.Gameplay.Player;
using Honeylab.Gameplay.SpeedUp;
using Honeylab.Gameplay.World;
using Honeylab.Persistence;
using Honeylab.Sounds;
using Honeylab.Utils;
using Honeylab.Utils.CameraTargeting;
using Honeylab.Utils.Configs;
using Honeylab.Utils.Extensions;
using Honeylab.Utils.Persistence;
using Honeylab.Utils.PushNotifications;
using Honeylab.Utils.Vfx;
using System;
using System.Collections.Generic;
using System.Threading;
using UniRx;
using UnityEngine;


namespace Honeylab.Gameplay.Buildings
{
    public class UnlockBuildingFlow : WorldObjectFlow, ISpeedUpAgent, IPriceFlow
    {
        [SerializeField] private ConfigIdProvider _configId;
        [SerializeField] private List<UnlockBuildingActionBase> _actions;

        private TimeService _time;

        private CancellationTokenSource _cts;
        private CompositeDisposable _disposable;

        private PlayerInputService _playerInput;


        public string ConfigId => _configId.Id;
        public ICameraTargetingService CameraTargeting { get; private set; }
        public VfxService Vfxs { get; private set; }
        public SoundService Sounds { get; private set; }
        public VibrationService Vibrations { get; private set; }
        public LevelPersistenceService LevelPersistenceService { get; private set; }
        public UnlockBuildingConfig Config { get; private set; }
        public UnlockBuildingStatePersistentComponent State { get; private set; }
        public UnlockBuildingConsumablesPersistentComponent Consumables { get; private set; }

        public UnlockTimePersistentComponent UnlockStartTime { get; private set; }
        public ISpeedUpAgentConfig SpeedUpConfig => Config;
        public float Duration => Config.UnlockDuration;
        public IReactiveProperty<double> TimeLeft { get; set; } = new ReactiveProperty<double>(-1.0);


        protected override void OnInit()
        {
            _time = Resolve<TimeService>();

            IConfigsService configs = Resolve<IConfigsService>();
            Config = configs.Get<UnlockBuildingConfig>(_configId.Id);

            _playerInput = Resolve<PlayerInputService>();
            CameraTargeting = Resolve<ICameraTargetingService>();
            Vfxs = Resolve<VfxService>();
            Sounds = Resolve<SoundService>();
            Vibrations = Resolve<VibrationService>();

            LevelPersistenceService = Resolve<LevelPersistenceService>();

            if (IsConsumablesProgressUnlock())
            {
                InitConsumablesProgressUnlock();
                return;
            }

            if (IsConsumablesUnlock())
            {
                InitConsumablesUnlock();
                return;
            }

            InitUnlock();
        }


        protected override void OnClear()
        {
            _disposable?.Dispose();
            _disposable = null;

            _cts?.CancelThenDispose();
            _cts = null;
        }


        public bool IsConsumablesProgressUnlock() => IsConsumablesUnlock() && IsProgressUnlock();
        public bool IsConsumablesUnlock() => Config.UnlockPrice is { Count: > 0 };
        public bool IsSingleConsumableUnlock() => IsConsumablesUnlock() && Config.UnlockPrice.Count == 1;
        public bool IsProgressUnlock() => Config.UnlockDuration > 0.0001f;


        private void InitConsumablesProgressUnlock()
        {
            if (LevelPersistenceService.TryGet(Id, out PersistentObject po))
            {
                State = po.GetOrAdd<UnlockBuildingStatePersistentComponent>();
                UnlockStartTime = po.GetOrAdd<UnlockTimePersistentComponent>();

                Consumables = po.GetOrAdd<UnlockBuildingConsumablesPersistentComponent>();
            }
            else
            {
                PersistentObject freshPo = LevelPersistenceService.Create(Id);
                State = freshPo.Add<UnlockBuildingStatePersistentComponent>();
                State.Value = UnlockBuildingStates.Idle;

                UnlockStartTime = freshPo.Add<UnlockTimePersistentComponent>();
                UnlockStartTime.Value = -1.0f;

                Consumables = freshPo.Add<UnlockBuildingConsumablesPersistentComponent>();
                Config.UnlockPrice.ForEach(_ => Consumables.Amounts.Add(0));
            }

            _cts = new CancellationTokenSource();
            _disposable = new CompositeDisposable();

            IPushNotificationService notifications = Resolve<IPushNotificationService>();
            IDisposable progress = State.ValueProperty.Pairwise()
                .Where(it => it is { Previous: UnlockBuildingStates.Idle, Current: UnlockBuildingStates.Progress })
                .Subscribe(_ =>
                {
                    float delay = Config.UnlockDuration;
                    notifications.RegisterUnlockNotification(Id, delay);
                });
            _disposable.Add(progress);

            CancellationToken ct = _cts.Token;
            IDisposable unlock = State.ValueProperty
                .Pairwise()
                .Where(it => it is { Previous: UnlockBuildingStates.Claim, Current: UnlockBuildingStates.Unlocked })
                .Subscribe(_ => UnlockAsync(ct).Forget());
            _disposable.Add(unlock);
        }


        private void InitConsumablesUnlock()
        {
            if (LevelPersistenceService.TryGet(Id, out PersistentObject po))
            {
                State = po.GetOrAdd<UnlockBuildingStatePersistentComponent>();
                Consumables = po.GetOrAdd<UnlockBuildingConsumablesPersistentComponent>();
            }
            else
            {
                PersistentObject freshPo = LevelPersistenceService.Create(Id);
                State = freshPo.Add<UnlockBuildingStatePersistentComponent>();
                State.Value = UnlockBuildingStates.Idle;

                Consumables = freshPo.Add<UnlockBuildingConsumablesPersistentComponent>();
                Config.UnlockPrice.ForEach(_ => Consumables.Amounts.Add(0));
            }

            _cts = new CancellationTokenSource();
            _disposable = new CompositeDisposable();

            CancellationToken ct = _cts.Token;
            IDisposable unlock = State.ValueProperty
                .Pairwise()
                .Where(it => it is { Previous: UnlockBuildingStates.Idle, Current: UnlockBuildingStates.Unlocked })
                .Subscribe(_ => UnlockAsync(ct).Forget());
            _disposable.Add(unlock);
        }


        private void InitUnlock()
        {
            if (LevelPersistenceService.TryGet(Id, out PersistentObject po))
            {
                State = po.GetOrAdd<UnlockBuildingStatePersistentComponent>();
            }
            else
            {
                PersistentObject freshPo = LevelPersistenceService.Create(Id);
                State = freshPo.Add<UnlockBuildingStatePersistentComponent>();
                State.Value = UnlockBuildingStates.Idle;
            }

            _cts = new CancellationTokenSource();
            _disposable = new CompositeDisposable();

            CancellationToken ct = _cts.Token;
            IDisposable unlock = State.ValueProperty
                .Pairwise()
                .Where(it => it is { Previous: UnlockBuildingStates.Idle, Current: UnlockBuildingStates.Unlocked })
                .Subscribe(_ => UnlockAsync(ct).Forget());
            _disposable.Add(unlock);
        }


        protected override async UniTask OnRunAsync(CancellationToken ct)
        {
            if (!IsConsumablesProgressUnlock())
            {
                return;
            }

            while (true)
            {
                await UniTask.WaitUntil(() => State.Value == UnlockBuildingStates.Progress, cancellationToken: ct);

                if (UnlockStartTime.Value < 0.0f)
                {
                    UnlockStartTime.Value = _time.GetUtcTime();
                }

                TimeLeft.Value = Config.UnlockDuration - (_time.GetUtcTime() - UnlockStartTime.Value);

                float lastTime = _time.GetRealtime();
                while (TimeLeft.Value > 0.0f)
                {
                    await UniTask.Yield(ct);

                    double timeLeft = TimeLeft.Value;
                    float deltaTime = _time.GetDeltaRealtime(lastTime);
                    timeLeft -= deltaTime;
                    TimeLeft.Value = timeLeft > 0.0f ? timeLeft : -1.0f;

                    lastTime += deltaTime;
                }

                State.Value = UnlockBuildingStates.Claim;
                UnlockStartTime.Value = -1.0;
            }
        }


        private async UniTask UnlockAsync(CancellationToken ct)
        {
            using (_playerInput.BlockInput())
            {
                foreach (UnlockBuildingActionBase action in _actions)
                {
                    await action.UnlockAsync(ct);
                }
            }
        }


        public void SpeedUp(float time)
        {
            double timeLeft = TimeLeft.Value;
            timeLeft -= time;
            TimeLeft.Value = timeLeft > 0.0f ? timeLeft : -1.0f;
        }


        public List<RewardAmountConfig> Price => Config.UnlockPrice;
    }
}
