using Cysharp.Threading.Tasks;
using Honeylab.Consumables.Configs;
using Honeylab.Gameplay.Buildings.Config;
using Honeylab.Gameplay.SimpleGet;
using Honeylab.Gameplay.SpeedUp;
using Honeylab.Gameplay.World;
using Honeylab.Persistence;
using Honeylab.Utils;
using Honeylab.Utils.Configs;
using Honeylab.Utils.Persistence;
using Honeylab.Utils.PushNotifications;
using System;
using System.Collections.Generic;
using System.Threading;
using UniRx;
using UnityEngine;


namespace Honeylab.Gameplay.Buildings
{
    public class CraftBuildingFlow : WorldObjectFlow, ISpeedUpAgent, ISimpleGetAgent, IPriceFlow
    {
        [SerializeField] private ConfigIdProvider _configId;

        private TimeService _time;
        private readonly CompositeDisposable _disposable = new();


        public string ConfigId => _configId.Id;
        public CraftBuildingConfig Config { get; private set; }

        public CraftStatePersistentComponent State { get; private set; }
        public CraftTimePersistentComponent CraftStartTime { get; private set; }
        public CraftAmountPersistentComponent CraftAmount { get; private set; }

        public ISpeedUpAgentConfig SpeedUpConfig => Config;
        public float Duration => Config.CraftDuration * CraftAmount.Value;
        public IReactiveProperty<double> TimeLeft { get; set; } = new ReactiveProperty<double>(-1.0);

        protected override void OnInit()
        {
            _time = Resolve<TimeService>();

            IConfigsService configs = Resolve<IConfigsService>();
            Config = configs.Get<CraftBuildingConfig>(_configId.Id);

            LevelPersistenceService persistence = Resolve<LevelPersistenceService>();
            if (!persistence.TryGet(Id, out PersistentObject outPo) || !outPo.Has<CraftStatePersistentComponent>())
            {
                PersistentObject po = outPo ?? persistence.Create(Id);
                State = po.Add<CraftStatePersistentComponent>();
                State.Value = CraftBuildingStates.Idle;

                CraftStartTime = po.Add<CraftTimePersistentComponent>();
                CraftStartTime.Value = -1.0f;

                CraftAmount = po.Add<CraftAmountPersistentComponent>();
                CraftAmount.Value = 1;
            }
            else
            {
                State = outPo.GetOrAdd<CraftStatePersistentComponent>();
                CraftStartTime = outPo.GetOrAdd<CraftTimePersistentComponent>();
                CraftAmount = outPo.GetOrAdd<CraftAmountPersistentComponent>();
            }

            IPushNotificationService notifications = Resolve<IPushNotificationService>();
            IDisposable work = State.ValueProperty.Pairwise()
                .Where(it => it is { Previous: CraftBuildingStates.Idle, Current: CraftBuildingStates.Work })
                .Subscribe(_ =>
                {
                    float delay = Config.CraftDuration * CraftAmount.Value;
                    notifications.RegisterCraftNotification(Id, delay);
                });
            _disposable.Add(work);
        }


        protected override void OnClear()
        {
            _disposable?.Dispose();
        }


        protected override async UniTask OnRunAsync(CancellationToken ct)
        {
            CraftBuildingViewBase view = Get<CraftBuildingViewBase>();
            while (true)
            {
                view.PlayIdle();

                await UniTask.WaitUntil(() => State.Value == CraftBuildingStates.Work, cancellationToken: ct);
                view.PlayWork();

                if (CraftStartTime.Value < 0.0f)
                {
                    CraftStartTime.Value = _time.GetUtcTime();
                }

                TimeLeft.Value = Config.CraftDuration * CraftAmount.Value -
                    (_time.GetUtcTime() - CraftStartTime.Value);

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

                State.Value = CraftBuildingStates.Done;
                CraftStartTime.Value = -1.0;
            }
        }


        public int GetCurrentAmount()
        {
            float currentAmount = (Config.CraftDuration * CraftAmount.Value - (float)TimeLeft.Value) / Config.CraftDuration;
            currentAmount = Mathf.Min(currentAmount, CraftAmount.Value);
            return Mathf.FloorToInt(currentAmount);
        }


        public void SpeedUp(float time)
        {
            double timeLeft = TimeLeft.Value;
            timeLeft -= time;
            TimeLeft.Value = timeLeft > 0.0f ? timeLeft : -1.0f;
        }


        public List<RewardAmountConfig> Price => Config.CraftPrice;
    }
}
