using Cysharp.Threading.Tasks;
using Honeylab.Gameplay.Config;
using Honeylab.Gameplay.Weapons;
using Honeylab.Gameplay.World;
using Honeylab.Utils;
using Honeylab.Utils.Configs;
using Honeylab.Utils.Extensions;
using System;
using System.Threading;
using UniRx;
using UnityEngine;


namespace Honeylab.Gameplay
{
    public class Recover : WorldObjectComponentBase
    {
        [SerializeField] private ConfigIdProvider _configId;
        [SerializeField] private Health _health;

        private TimeService _timeService;

        private Die _die;
        private WeaponAttackTargetView _weaponAttackTargetView;
        private float _cooldown = -1.0f;

        private RecoverConfig _config;
        private CancellationTokenSource _run;
        private CancellationTokenSource _showHealthPopup;
        private CompositeDisposable _disposable;


        protected override void OnInit()
        {
            WorldObjectFlow flow = GetFlow();
            _timeService = flow.Resolve<TimeService>();
            _die = flow.Get<Die>();
            _weaponAttackTargetView = flow.Get<WeaponAttackTargetView>();

            IConfigsService configs = flow.Resolve<IConfigsService>();
            _config = configs.Get<RecoverConfig>(_configId.Id);
        }


        protected override void OnRun()
        {
            _run = new CancellationTokenSource();
            _disposable = new CompositeDisposable();

            CancellationToken ct = _run.Token;
            IDisposable damaged = _health.HealthProp.Pairwise()
                .Where(hp => hp.Current < hp.Previous)
                .Subscribe(_ => { _cooldown = _config.RecoverCooldownDamaged; });
            _disposable.Add(damaged);

            RunRecoverCooldownLoopAsync(ct).Forget();
            RunRecoverLoopAsync(ct).Forget();
        }


        private async UniTask RunRecoverCooldownLoopAsync(CancellationToken ct)
        {
            while (true)
            {
                await UniTask.Yield(ct);

                if (_die != null && _die.IsDead)
                {
                    continue;
                }

                if (_cooldown > 0.0f)
                {
                    _cooldown -= _timeService.GetDeltaTime();
                }
            }
        }


        protected override void OnStop()
        {
            _disposable?.Dispose();
            _disposable = null;

            _showHealthPopup?.CancelThenDispose();
            _showHealthPopup = null;

            _run?.CancelThenDispose();
            _run = null;
        }


        private async UniTask RunRecoverLoopAsync(CancellationToken ct)
        {
            while (true)
            {
                if (_die != null)
                {
                    await UniTask.WaitUntil(() => !_die.IsDead, cancellationToken: ct);
                }

                await UniTask.WaitUntil(() => _cooldown <= 0.0f, cancellationToken: ct);

                await UniTask.Delay(TimeSpan.FromSeconds(_config.RecoverTime), cancellationToken: ct);

                if (_die is { IsDying: true } || _cooldown > 0.0f)
                {
                    continue;
                }

                float maxHealth = _health.MaxHealth;
                if (_health.HealthProp.Value < maxHealth)
                {
                    float health = _health.HealthProp.Value +
                        _config.RecoverAmountInPercents * (_health.HealthProp.Value / 100.0f);
                    _health.HealthProp.Value = Mathf.Clamp(health, 0.0f, maxHealth);

                    ShowHealthPopup();
                }
            }
        }


        private void ShowHealthPopup()
        {
            _showHealthPopup?.CancelThenDispose();
            _showHealthPopup = new();

            if (_weaponAttackTargetView != null)
            {
                _weaponAttackTargetView.ShowHealthDurationPopupAsync(_showHealthPopup.Token).Forget();
            }
        }
    }
}
