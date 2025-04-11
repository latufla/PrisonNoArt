using Cysharp.Threading.Tasks;
using Honeylab.Gameplay.Player;
using Honeylab.Gameplay.Weapons;
using Honeylab.Gameplay.World;
using Honeylab.Persistence;
using Honeylab.Utils;
using Honeylab.Utils.Configs;
using Honeylab.Utils.Extensions;
using Honeylab.Utils.Persistence;
using System;
using System.Linq;
using System.Threading;
using UniRx;
using UnityEngine;


namespace Honeylab.Gameplay.Booster
{
    public class BoosterService : IDisposable
    {
        private readonly WorldObjectsService _world;
        private readonly IConfigsService _configsService;
        private readonly string _boosterConfigId;
        private readonly PersistenceId _boosterId;
        private readonly SharedPersistenceService _sharedPersistenceService;
        private readonly TimeService _time;
        private readonly Subject<bool> _onDamageBoosted = new Subject<bool>();

        private CancellationTokenSource _cts;
        private const int DamageBoostMultiplier = 3;
        private BoosterConfig _boosterConfig;
        private float _activeTimeLeft;
        private float _showTimeLeft;

        public DamageBoostPersistentComponent DamageBoostPersistentComponent { get; private set; }
        public int GetDamageBoost() => IsDamageBoosted() ? DamageBoostMultiplier : 1;
        public bool IsDamageBoostAvailable() => _showTimeLeft > 0;
        public bool IsDamageBoosted() => _activeTimeLeft > 0;
        public float GetActiveTimeLeft() => _activeTimeLeft;
        public float GetActiveTimeDuration() => _boosterConfig.ActiveTime;
        public PersistenceId GetBoosterId() => _boosterId;
        public IObservable<bool> OnDamageBoostedAsObservable() => _onDamageBoosted.AsObservable();
        public bool IsRunning { get; private set; }


        public BoosterService(PersistenceId boosterId,
            string boosterConfigId,
            WorldObjectsService world,
            IConfigsService configsService,
            SharedPersistenceService sharedPersistenceService,
            TimeService time)
        {
            _boosterId = boosterId;
            _boosterConfigId = boosterConfigId;
            _world = world;
            _configsService = configsService;
            _sharedPersistenceService = sharedPersistenceService;
            _time = time;
        }


        public void Init()
        {
            if (!_sharedPersistenceService.TryGet(_boosterId, out PersistentObject boosterPo) ||
                !boosterPo.Has<DamageBoostPersistentComponent>())
            {
                PersistentObject po = boosterPo ?? _sharedPersistenceService.Create(_boosterId);
                DamageBoostPersistentComponent = po.Add<DamageBoostPersistentComponent>();
                DamageBoostPersistentComponent.StartBoostActiveTime = -1.0f;
                DamageBoostPersistentComponent.StartBoostShowTime = -1.0f;
            }
            else
            {
                DamageBoostPersistentComponent = boosterPo.GetOrAdd<DamageBoostPersistentComponent>();
            }

            _boosterConfig = _configsService.Get<BoosterConfig>(_boosterConfigId);
        }


        public async UniTask RunAsync()
        {
            _cts = new CancellationTokenSource();
            CancellationToken ct = _cts.Token;

            await WaitWeaponAgent(ct);
            IsRunning = true;

            int startDelay = DamageBoostPersistentComponent.ItWasStarting ?
                _boosterConfig.StartDelay :
                _boosterConfig.FirstDelay;

            bool firstLaunch = SetBoosterTime();
            startDelay = firstLaunch ? startDelay : 0;

            await UniTask.Delay(TimeSpan.FromSeconds(startDelay), cancellationToken: ct);

            while (true)
            {
                await UniTask.Yield(ct);

                float timeLeft = startDelay < 0 ? _boosterConfig.TimeToAvailability : 0;
                while (timeLeft > 0)
                {
                    await UniTask.Yield(ct);
                    timeLeft -= Time.deltaTime;
                }

                startDelay = -1;
                await WorkAsync(ct);
            }
        }


        private async UniTask WorkAsync(CancellationToken ct)
        {
            DamageBoostPersistentComponent.ItWasStarting = true;

            DamageBoostPersistentComponent.StartBoostShowTime =
                DamageBoostPersistentComponent.StartBoostShowTime <= 0 ?
                    _time.GetUtcTime() :
                    DamageBoostPersistentComponent.StartBoostShowTime;

            _showTimeLeft = _showTimeLeft <= 0 ? _boosterConfig.ShowTime : _showTimeLeft;
            while (_showTimeLeft > 0 || _activeTimeLeft > 0)
            {
                await UniTask.Yield(ct);
                _showTimeLeft -= Time.deltaTime;

                if (_activeTimeLeft <= 0)
                {
                    continue;
                }

                DamageBoostPersistentComponent.StartBoostActiveTime =
                    DamageBoostPersistentComponent.StartBoostActiveTime <= 0 ?
                        _time.GetUtcTime() :
                        DamageBoostPersistentComponent.StartBoostActiveTime;

                DamageBoostPersistentComponent.StartBoostShowTime = -1.0f;
                DamageBoostPersistentComponent.IsFirstDamageBoost = true;

                _activeTimeLeft = _activeTimeLeft <= 0 ? _boosterConfig.ActiveTime : _activeTimeLeft;
                _onDamageBoosted.OnNext(true);

                while (_activeTimeLeft > 0)
                {
                    await UniTask.Yield(ct);
                    _activeTimeLeft -= Time.deltaTime;
                }

                _onDamageBoosted.OnNext(false);
                _activeTimeLeft = 0;
                break;
            }

            DamageBoostPersistentComponent.StartBoostShowTime = -1.0f;
            DamageBoostPersistentComponent.StartBoostActiveTime = -1.0f;
            _showTimeLeft = 0;
        }


        private bool SetBoosterTime()
        {
            double elapsedTime = DamageBoostPersistentComponent.StartBoostActiveTime > 0 ?
                _time.GetUtcTime() - DamageBoostPersistentComponent.StartBoostActiveTime :
                -1.0f;

            if (elapsedTime > 0)
            {
                _activeTimeLeft = _boosterConfig.ActiveTime - (float)elapsedTime;
                if (_activeTimeLeft > 0 && _activeTimeLeft < _boosterConfig.ActiveTime)
                {
                    return false;
                }

                _activeTimeLeft = 0;
                DamageBoostPersistentComponent.StartBoostActiveTime = -1.0f;
            }

            elapsedTime = DamageBoostPersistentComponent.StartBoostShowTime > 0 ?
                _time.GetUtcTime() - DamageBoostPersistentComponent.StartBoostShowTime :
                -1.0f;

            if (elapsedTime > 0)
            {
                _showTimeLeft = _boosterConfig.ShowTime - (float)elapsedTime;
                if (_showTimeLeft > 0 && _showTimeLeft < _boosterConfig.ShowTime)
                {
                    return false;
                }

                _showTimeLeft = 0;
                DamageBoostPersistentComponent.StartBoostShowTime = -1.0f;
            }

            return true;
        }


        private async UniTask WaitWeaponAgent(CancellationToken ct)
        {
            WeaponAgent weaponAgent;
            await UniTask.WaitUntil(() =>
                {
                    PlayerFlow player = _world.GetObjects<PlayerFlow>().FirstOrDefault();
                    if (player == null)
                    {
                        return false;
                    }

                    weaponAgent = player.Get<WeaponAgent>();
                    return weaponAgent != null;
                },
                cancellationToken: ct);
        }


        public void DamageBoost()
        {
            if (IsDamageBoostAvailable())
            {
                _activeTimeLeft = _boosterConfig.ActiveTime;
            }
        }


        public void Dispose()
        {
            _cts?.CancelThenDispose();
            _cts = null;
        }
    }
}
