using Cysharp.Threading.Tasks;
using Honeylab.Ads;
using Honeylab.Gameplay.Booster;
using Honeylab.Gameplay.Ui.Upgrades;
using Honeylab.Utils;
using System;
using System.Threading;
using UniRx;
using UnityEngine;
using UnityEngine.UI;


namespace Honeylab.Gameplay.Ui.Booster
{
    public class WeaponBoosterPanel : MonoBehaviour
    {
        [SerializeField] private RectTransform _layout;
        [SerializeField] private Image _icon;
        [SerializeField] private TimeProgressPanel _timeProgressPanel;

        private BoosterService _boosterService;
        private WeaponUpgradeStatusPanel _weaponUpgradeStatusPanel;
        private WeaponBoosterButton _weaponBoosterButton;
        private RewardedAdsService _rewardedAdsService;


        private const string Free = "FREE";
        private const string WatchAd = "WATCH AD";


        public void Init(BoosterService boosterService,
            WeaponUpgradeStatusPanel weaponUpgradeStatusPanel,
            WeaponBoosterButton weaponBoosterButton,
            RewardedAdsService rewardedAdsService)
        {
            _boosterService = boosterService;
            _weaponUpgradeStatusPanel = weaponUpgradeStatusPanel;
            _weaponBoosterButton = weaponBoosterButton;
            _rewardedAdsService = rewardedAdsService;
        }


        public async UniTask RunAsync(CancellationToken ct, CompositeDisposable disposable)
        {
            _weaponBoosterButton.RvButton.Init(_rewardedAdsService, _boosterService.GetBoosterId(), RewardedAdsLocation.Booster);
            _weaponBoosterButton.RvButton.Run();

            SetActive(false);
            _weaponBoosterButton.FreeButton.SetActive(false);
            _weaponBoosterButton.RvButton.SetActive(false);

            IDisposable onWeaponBoost = _weaponBoosterButton.RvButton.OnRewardedAdShownAsObservable()
                .Subscribe(result =>
                {
                    if (result.State != RewardedAdResultState.Success)
                    {
                        return;
                    }
                    _boosterService.DamageBoost();
                });
            disposable.Add(onWeaponBoost);

            IDisposable onWeaponBoostFree = _weaponBoosterButton.FreeButton.OnButtonClickAsObservable()
                .Where(_ => !_boosterService.DamageBoostPersistentComponent.IsFirstDamageBoost)
                .Subscribe(_ =>
                {
                    _boosterService.DamageBoost();
                });
            disposable.Add(onWeaponBoostFree);

            await UniTask.WaitUntil(() => _boosterService.IsRunning, cancellationToken: ct);

            while (!ct.IsCancellationRequested)
            {
                await UniTask.Yield(ct);

                if (!_boosterService.IsDamageBoostAvailable())
                {
                    SetActive(false);
                    _weaponBoosterButton.FreeButton.SetActive(false);
                    _weaponBoosterButton.RvButton.SetActive(false);
                    _weaponBoosterButton.SetActive(false);
                }

                await UniTask.WaitUntil(() => _boosterService.IsDamageBoostAvailable(), cancellationToken: ct);

                bool freeButtonActive = !_boosterService.DamageBoostPersistentComponent.IsFirstDamageBoost;
                _weaponBoosterButton.FreeButton.SetActive(freeButtonActive);
                _weaponBoosterButton.RvButton.SetActive(!freeButtonActive);
                _weaponBoosterButton.SetActive(true);

                while (_boosterService.IsDamageBoostAvailable())
                {
                    await UniTask.Yield(ct);

                    Sprite weaponIcon = _weaponUpgradeStatusPanel.GetIconSprite();
                    if (freeButtonActive)
                    {
                        _weaponBoosterButton.FreeButton.SetIcon(weaponIcon);
                        _weaponBoosterButton.FreeButton.SetLabel(Free);
                    }
                    else
                    {
                        _weaponBoosterButton.RvButton.SetIcon(weaponIcon);
                        _weaponBoosterButton.RvButton.SetLabel(WatchAd);
                    }

                    while (_boosterService.IsDamageBoosted())
                    {
                        weaponIcon = _weaponUpgradeStatusPanel.GetIconSprite();
                        SetActive(true);
                        SetTime(_boosterService.GetActiveTimeLeft(), _boosterService.GetActiveTimeDuration());
                        SetIcon(weaponIcon);

                        _weaponBoosterButton.FreeButton.SetActive(false);
                        _weaponBoosterButton.RvButton.SetActive(false);
                        _weaponBoosterButton.SetActive(false);

                        await UniTask.Yield(ct);
                    }
                }
            }
        }


        public void SetIcon(Sprite sprite)
        {
            _icon.sprite = sprite;
        }


        public void SetTime(float timeLeft, float duration)
        {
            _timeProgressPanel.SetTime(timeLeft, duration);
        }


        public void SetActive(bool isEnabled)
        {
            gameObject.SetActive(isEnabled);
            LayoutRebuilder.ForceRebuildLayoutImmediate(_layout);
        }
    }
}
