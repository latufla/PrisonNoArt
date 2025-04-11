using Cysharp.Threading.Tasks;
using DG.Tweening;
using Honeylab.Gameplay.Creatures;
using Honeylab.Gameplay.Pools;
using Honeylab.Gameplay.Ui;
using Honeylab.Gameplay.Weapons.Upgrades;
using Honeylab.Gameplay.World;
using Honeylab.Sounds;
using Honeylab.Sounds.Data;
using Honeylab.Utils;
using Honeylab.Utils.Configs;
using Honeylab.Utils.Extensions;
using Honeylab.Utils.Formatting;
using Honeylab.Utils.Vfx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UniRx;
using UnityEngine;
using Random = UnityEngine.Random;


namespace Honeylab.Gameplay.Weapons
{
    public class ShakeConfig
    {
        public float TotalDuration { get; set; } // Total duration of the wave effect
        public float MoveFrequency { get; set; } // Frequency of the wave oscillation
        public float InitialAmplitude { get; set; } // Starting amplitude of the wave
        public float MoveToSideAmplitude { get; set; } // Maximum offset to the opposite side
    }


    public class WeaponAttackTargetView : WorldObjectComponentVisual
    {
        [SerializeField] private ConfigIdProvider _configId;
        [SerializeField] private Health _health;
        [SerializeField] private List<GameObject> _states;
        [SerializeField] private GameObject _skin;

        private GameObject _currentState;

        [SerializeField] private HealthPopupView _healthPopupView;
        [SerializeField] private float _hitShowHealthPopupDuration = 2.0f;
        [SerializeField] private string _hitNotEnoughLevelText = "Need level {0}";
        [SerializeField] private Transform _hitNotEnoughLevelToastAnchor;
        [SerializeField] private bool _hitDamageToast = true;
        [SerializeField] private Vector3 _hitDamageToastHealthPopupOffset = Vector3.up;
        [SerializeField] private float _hitDamageToastSpread = 1.0f;
        [SerializeField] private bool _showHealthAmountText;
        [SerializeField] private bool _isBounceShake;

        private ToastsService _toasts;

        private WorldObjectFlow _flow;
        private WeaponAttackTargetConfig _config;
        private VfxService _vfxs;
        private SoundService _soundService;
        private CancellationTokenSource _hitReactionCts;
        private HealthPopup _healthPopup;
        private readonly Subject<WeaponFlow> _onStateChanged = new();
        private VibrationService _vibrationService;
        private bool _keepHealthPopupUntilHide;

        public IObservable<WeaponFlow> OnStateChanged => _onStateChanged;
        public int StatesCount => _states.Count;
        public WeaponAttackTargetConfig Config => _config;

        private const string SHAKE_CONFIG = "ShakeConfig";
        private ShakeConfig _shakeConfig;


        protected override void OnInit()
        {
            _flow = GetFlow();
            _toasts = _flow.Resolve<ToastsService>();

            IConfigsService configs = _flow.Resolve<IConfigsService>();
            _config = configs.Get<WeaponAttackTargetConfig>(_configId.Id);
            _shakeConfig = configs.Get<ShakeConfig>(SHAKE_CONFIG);
            _vfxs = _flow.Resolve<VfxService>();
            _soundService = _flow.Resolve<SoundService>();
            _vibrationService = _flow.Resolve<VibrationService>();

            BillboardPresenterFactory billboards = _flow.Resolve<BillboardPresenterFactory>();
            GameplayPoolsService pools = _flow.Resolve<GameplayPoolsService>();
            _healthPopupView.Init(pools, billboards);

            transform.localPosition = Vector3.zero;
        }


        protected override void OnClear()
        {
            _hitReactionCts?.CancelThenDispose();
            _hitReactionCts = null;

            HideHealthPopup();

            base.OnClear();
        }


        public void ChangeState(float health, WeaponFlow weapon = null)
        {
            if (_states != null && _states.Count > 0)
            {
                _states.ForEach(it => it.SetActive(false));

                float ratio = health / _health.MaxHealth;
                int stateIndex = Mathf.CeilToInt(Mathf.Lerp(0, _states.Count - 1, ratio));
                _states[stateIndex].SetActive(true);

                bool isMaxState = stateIndex == _states.Count - 1;
                bool isFirstIteration = _currentState == null;

                var oldState = _currentState != null ? _currentState : _states[stateIndex];
                _currentState = _states[stateIndex];

                if (oldState != _currentState || (!isMaxState && isFirstIteration))
                {
                    if (isFirstIteration)
                    {
                        var completedStages = _states.Count - 1 - stateIndex;
                        for (int i = 0; i < completedStages; i++)
                        {
                            _onStateChanged.OnNext(weapon);
                        }
                    }
                    else
                    {
                        _onStateChanged.OnNext(weapon);
                    }
                }
            }
        }


        public void UpdateHealthView()
        {
            if (_healthPopup != null)
            {
                _healthPopup.HealthBarView.SetHealth(_health.HealthProp.Value,
                    _health.MaxHealth,
                    true,
                    _showHealthAmountText);
            }
        }


        public void PlayHitReaction(bool success,
            WeaponFlow weapon,
            int damage,
            bool showNotEnoughLevelIfNeed = true)
        {
            _hitReactionCts?.CancelThenDispose();
            _hitReactionCts = new CancellationTokenSource();

            WeaponAttackBase attack = weapon.Get<WeaponAttackBase>();

            CancellationToken ct = _hitReactionCts.Token;
            if (success)
            {
                Knockback knockback = GetKnockback(weapon);
                if (knockback != null)
                {
                    knockback.PlayKnockback(weapon);
                }
                else
                {
                    PlayShake(weapon, ct);
                }

                ChangeColorState(inDuration: 0, outDuration: 0.6f, Ease.InSine, ct);
                _vfxs.PlayOnceAsync(attack.HitVfxId, transform).Forget();

                ShowHealthDurationPopupAsync(ct).Forget();
                ShowDamageToast(damage);
            }
            else if (showNotEnoughLevelIfNeed)
            {
                PlayShake(weapon, ct);

                _toasts.ShowTextToast(_hitNotEnoughLevelToastAnchor.position,
                    string.Format(_hitNotEnoughLevelText, _config.WeaponLevel));
            }

            _soundService.RequestSoundPlay(attack.HitSoundId);
            _vibrationService.Vibrate();
        }


        [Obsolete("Asign from Inteface HitVfxId and HitSoundId and remove this method")]
        public void PlayHitReactionTest(bool success, int damage, bool changeColor, VfxId hitVfxId = null, SoundId hitSoundId = null, bool showNotEnoughLevelIfNeed = true)
        {
            _hitReactionCts?.CancelThenDispose();
            _hitReactionCts = new CancellationTokenSource();

            CancellationToken ct = _hitReactionCts.Token;

            if (success)
            {
                if (changeColor)
                {
                    ChangeColorState(inDuration: 0, outDuration: 0.4f, Ease.InSine, ct);
                }

                if (hitVfxId != null)
                {
                    _vfxs.PlayOnceAsync(hitVfxId, transform).Forget();
                }


                ShowHealthDurationPopupAsync(ct).Forget();
                ShowDamageToast(damage);
            }
            else if (showNotEnoughLevelIfNeed)
            {
                _toasts.ShowTextToast(_hitNotEnoughLevelToastAnchor.position,
                    string.Format(_hitNotEnoughLevelText, _config.WeaponLevel));
            }

            if (hitSoundId != null)
            {
                _soundService.RequestSoundPlay(hitSoundId);
            }

            _vibrationService.Vibrate();
        }


        protected override List<Renderer> GetSkinRenderers()
        {
            if (_skin != null)
            {
                return _skin.GetComponentsInChildren<Renderer>().ToList();
            }

            if (_currentState == null)
            {
                return null;
            }

            if (!_currentState.TryGetComponent(out Renderer rend))
            {
                if (_currentState.transform.childCount <= 0)
                {
                    return null;
                }

                if (!_currentState.transform.GetChild(0).TryGetComponent(out rend))
                {
                    return null;
                }
            }

            return new List<Renderer> { rend };
        }


        public async UniTask ShowHealthDurationPopupAsync(CancellationToken ct)
        {
            if (_keepHealthPopupUntilHide)
            {
                _healthPopup.HealthBarView.SetHealth(_health.HealthProp.Value,
                    _health.MaxHealth,
                    false,
                    _showHealthAmountText);
                return;
            }

            ShowHealthPopup(false);

            if (_health.HealthProp.Value > 0)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(_hitShowHealthPopupDuration), cancellationToken: ct);
            }

            HideHealthPopup();
        }


        public void ShowHealthPopup(bool keepUntilHide = true)
        {
            _keepHealthPopupUntilHide = keepUntilHide;

            _healthPopup ??= _healthPopupView.Show();

            _healthPopup.HealthBarView.SetHealthInitial(_health.HealthProp.Value,
                _health.MaxHealth,
                _showHealthAmountText);
        }


        public void HideHealthPopup()
        {
            if (_healthPopup != null)
            {
                _healthPopup.Clear();
                _healthPopup = null;

                _healthPopupView.HideAsync(true).Forget();
            }

            _keepHealthPopupUntilHide = false;
        }


        private void PlayShake(WeaponFlow weapon, CancellationToken ct)
        {
            WeaponUpgradeLevelConfig config = weapon.UpgradeConfigProp.Value;
            if (config.ShakePower < 0.001f || config.ShakeDuration < 0.001f)
            {
                return;
            }

            Vector3 fromPosition = weapon.GetAgent().Transform.position;
            Vector3 direction = transform.position - fromPosition;
            direction.y = 0.0f;
            direction.Normalize();

            if (_isBounceShake)
            {
                Vector3 sourceEulerAngles = transform.eulerAngles;

                float elapsedTime = 0f;
                float totalDuration = _shakeConfig.TotalDuration;
                float waveFrequency = _shakeConfig.MoveFrequency;
                float initialAmplitude = _shakeConfig.InitialAmplitude;
                float moveToSideAmplitude = _shakeConfig.MoveToSideAmplitude;

                DOTween.To(() => elapsedTime, x => elapsedTime = x, totalDuration, totalDuration)
                    .OnUpdate(() =>
                    {
                        // Calculate the current fading amplitude
                        float currentAmplitude = Mathf.Lerp(initialAmplitude, 0f, elapsedTime / totalDuration);

                        // Calculate the oscillation from the sine wave
                        float waveRotation = Mathf.Sin(elapsedTime * waveFrequency) * currentAmplitude;

                        // Gradually move the baseline to the opposite side
                        float baselineShift = Mathf.Lerp(0f, moveToSideAmplitude, elapsedTime / totalDuration);

                        // Combine the wave oscillation with the baseline shift
                        float finalRotation = waveRotation + baselineShift;

                        transform.rotation = Quaternion.Euler(finalRotation, sourceEulerAngles.y, sourceEulerAngles.z);
                    })
                    .SetEase(Ease.OutBounce)
                    .ToUniTask(cancellationToken: ct)
                    .Forget();
            }
            else
            {
                Vector3 shake = direction * config.ShakePower;

                DOTween.Shake(() => transform.position,
                        x => transform.position = x,
                        config.ShakeDuration,
                        shake)
                    .ToUniTask(cancellationToken: ct)
                    .Forget();
                ;
            }
        }


        private Knockback GetKnockback(WeaponFlow weapon)
        {
            WeaponUpgradeLevelConfig config = weapon.UpgradeConfigProp.Value;
            if (config.KnockbackPower < 0.001f || config.KnockbackDuration < 0.001f)
            {
                return null;
            }

            Knockback knockback = _flow.Get<Knockback>();
            return knockback;
        }


        private void ShowDamageToast(int damage)
        {
            if (!_hitDamageToast)
            {
                return;
            }

            string text = AcronymedPrint.ToString(damage);

            Vector3 position = _healthPopupView.transform.position + _hitDamageToastHealthPopupOffset;
            TextToast toast = _toasts.ShowDamageToast(position, text);

            float rnd = Random.Range(0.0f, 1.0f);
            Vector3 direction = toast.transform.right;
            float spread = Mathf.Lerp(-_hitDamageToastSpread, _hitDamageToastSpread, rnd);
            toast.transform.position += direction * spread;
        }
    }
}
