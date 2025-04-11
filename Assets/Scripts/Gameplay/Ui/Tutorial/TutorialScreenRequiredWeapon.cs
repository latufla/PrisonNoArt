using Cysharp.Threading.Tasks;
using Honeylab.Gameplay.Buildings;
using Honeylab.Gameplay.Player;
using Honeylab.Gameplay.Upgrades;
using Honeylab.Gameplay.Weapons;
using Honeylab.Gameplay.World;
using Honeylab.Utils;
using Honeylab.Utils.Arrows;
using Honeylab.Utils.Data;
using Honeylab.Utils.Extensions;
using Honeylab.Utils.OffscreenTargetIndicators;
using Honeylab.Utils.Tutorial;
using System;
using System.Linq;
using System.Threading;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Honeylab.Gameplay.Tutorial
{
    public class TutorialScreenRequiredWeapon : TutorialScreenRequiredItem
    {
        [SerializeField] private WorldObjectId _meleeWeaponTypeId;
        [SerializeField] private GameObject _weaponStatusObj;
        [SerializeField] private TextMeshProUGUI _labelText;
        [SerializeField] private Image _weaponIcon;
        [SerializeField] private Color _labelColorSuitable;
        [SerializeField] private Color _labelColorNotSuitable;
        [SerializeField] private GameObject _checkmark;
        [SerializeField] private TimeProgressPanel _timeProgressPanel;

        private string _labelTextFormat;

        private WorldObjectsService _world;
        private ArrowsPool _arrowsPool;
        private WeaponsData _weaponsData;
        private OffscreenIndicatorsService _offscreenIndicatorsService;
        private TutorialScreenFocus _screenFocus;
        private OffscreenIndicator _upgradeBuildingOffscreenIndicator;
        private ScriptableId _offscreenIndicatorId;

        private CancellationTokenSource _cts;
        private CompositeDisposable _disposable;
        private IArrowHandle _arrow;
        private bool _isCompleted = false;
        public override bool IsCompleted() => _isCompleted;

        public IObservable<Unit> OnRequiredItemsButtonClickAsObservable() =>
            Button.OnClickAsObservable();

        public override void Init(TutorialFlow flow,
            TutorialScreenFocus screenFocus)
        {
            _world = flow.Resolve<WorldObjectsService>();
            _arrowsPool = flow.Resolve<ArrowsPool>();
            _weaponsData = flow.Resolve<WeaponsData>();
            _offscreenIndicatorsService = flow.Resolve<OffscreenIndicatorsService>();
            _screenFocus = screenFocus;
            _offscreenIndicatorId = flow.OffscreenIndicatorId;
        }

        public override void Run(TutorialInfo tutorialInfo)
        {
            _disposable?.Dispose();

            if (tutorialInfo.RequiredWeaponLevel <= 0)
            {
                return;
            }

            _disposable = new CompositeDisposable();

            Root.SetActive(true);

            ShowRequiredWeapon(tutorialInfo.RequiredWeaponLevel);
        }

        private void ShowRequiredWeapon(int requiredLevel)
        {
            PlayerFlow player = _world.GetObjects<PlayerFlow>().First();
            WorldObjectId weaponId = player.Get<WeaponAgent>().GetWeaponByTypeFirstOrDefault(_meleeWeaponTypeId);
            UpgradeFlow upgrade = _world.GetObject<UpgradeFlow>(weaponId);

            if (upgrade == null)
            {
                return;
            }

            int weaponLevel = upgrade.UpgradeLevelPersistence.Value;
            _weaponStatusObj.SetActive(true);
            _timeProgressPanel.gameObject.SetActive(false);
            WeaponData weaponData = _weaponsData.GetData(weaponId);
            WeaponLevelData weaponLevelData = weaponData.Levels[requiredLevel - 1];

            weaponLevel += 1;
            SetRequiredWeapon(weaponLevel, requiredLevel, weaponLevelData);

            UpgradeBuildingFlow upgradeBuilding = _world.GetObjects<UpgradeBuildingFlow>()
                .FirstOrDefault(it => it.WeaponId.Equals(weaponId));

            if (upgradeBuilding == null || upgradeBuilding.WeaponUpgradeConfig == null)
            {
                return;
            }

            if (weaponLevel < requiredLevel)
            {
                _upgradeBuildingOffscreenIndicator =
                    _offscreenIndicatorsService.Add(_offscreenIndicatorId, upgradeBuilding.transform);
                _upgradeBuildingOffscreenIndicator.SetIcon(weaponLevelData.Sprite);
            }

            IDisposable updateProgress = upgradeBuilding.TimeLeft.Subscribe(timeLeft =>
            {
                _weaponStatusObj.SetActive(false);
                _timeProgressPanel.gameObject.SetActive(true);
                _timeProgressPanel.SetTime((float)timeLeft, upgradeBuilding.WeaponUpgradeConfig.Duration);
            });
            _disposable.Add(updateProgress);

            IDisposable updateStatus = upgradeBuilding.State.ValueProperty.Subscribe(state =>
            {
                if (state == UpgradeBuildingStates.Done)
                {
                    _weaponStatusObj.SetActive(true);
                    _timeProgressPanel.gameObject.SetActive(false);
                }
            });
            _disposable.Add(updateStatus);

            IDisposable onUpdateWeapon = upgrade.UpgradeLevelPersistence.ValueProperty
                .Subscribe(level =>
                {
                    level += 1;
                    SetRequiredWeapon(level, requiredLevel, weaponLevelData);
                    if (level >= requiredLevel && _upgradeBuildingOffscreenIndicator != null)
                    {
                        _offscreenIndicatorsService.Remove(_upgradeBuildingOffscreenIndicator);
                        _upgradeBuildingOffscreenIndicator = null;
                    }
                });
            _disposable.Add(onUpdateWeapon);

            IDisposable onRequiredPanelFocus = OnRequiredItemsButtonClickAsObservable()
                .Where(_ => upgrade.UpgradeLevelPersistence.Value + 1 < requiredLevel)
                .Subscribe(_ => { _screenFocus.FocusTarget(upgradeBuilding.transform); });
            _disposable.Add(onRequiredPanelFocus);

            if (weaponLevel < requiredLevel)
            {
                ArrowWorkAsync(upgradeBuilding, upgrade, requiredLevel).Forget();
            }
        }

        private void SetRequiredWeapon(int level, int requiredLevel, WeaponLevelData weaponLevelData)
        {
            Root.SetActive(requiredLevel > 0);

            _weaponStatusObj.SetActive(true);
            _timeProgressPanel.gameObject.SetActive(false);
            if (requiredLevel > 0)
            {
                _isCompleted = level >= requiredLevel;
                _weaponIcon.sprite = weaponLevelData.Sprite;
                _labelText.color = _isCompleted ?
                    _labelColorSuitable :
                    _labelColorNotSuitable;
                _checkmark.SetActive(_isCompleted);

                if (string.IsNullOrEmpty(_labelTextFormat))
                {
                    _labelTextFormat = _labelText.text;
                }

                string levelText = requiredLevel.ToString();
                _labelText.text = string.Format(_labelTextFormat, levelText);

            }
        }

        private async UniTask ArrowWorkAsync(UpgradeBuildingFlow upgradeBuilding,
           UpgradeFlow upgrade,
           int requiredLevel)
        {
            _cts?.CancelThenDispose();
            _cts = new CancellationTokenSource();
            CancellationToken ct = _cts.Token;

            var upgradeInteractable = upgradeBuilding.Get<UpgradeBuildingInteractable>();
            while (true)
            {
                if (upgrade.UpgradeLevelPersistence.Value + 1 >= requiredLevel)
                {
                    break;
                }
                await UniTask.Yield(ct);
                _arrow = ShowTargetArrow(upgradeBuilding.transform, 6);
                await upgradeInteractable.OnEnterInteractAsObservable().ToUniTask(true, cancellationToken: ct);
                _arrowsPool.HideAsync(_arrow).Forget();
                _arrow = null;
                await upgradeInteractable.OnExitInteractAsObservable().ToUniTask(true, cancellationToken: ct);
            }

            if (_arrow != null)
            {
                _arrowsPool.HideAsync(_arrow).Forget();
                _arrow = null;
            }
        }


        private IArrowHandle ShowTargetArrow(Transform target, float offsetY)
        {
            Vector3 arrowPosition = target.position;
            arrowPosition.y += offsetY;
            IArrowHandle arrow = _arrowsPool.PopAndShowArrow(arrowPosition);
            return arrow;
        }

        public override void Hide()
        {
            if (_upgradeBuildingOffscreenIndicator != null)
            {
                _offscreenIndicatorsService.Remove(_upgradeBuildingOffscreenIndicator);
                _upgradeBuildingOffscreenIndicator = null;
            }

            Root.SetActive(false);
            Button.gameObject.SetActive(false);

            _cts?.CancelThenDispose();
            _cts = null;

            _disposable?.Dispose();
            _disposable = null;

            if (_arrow != null)
            {
                _arrowsPool.HideAsync(_arrow).Forget();
                _arrow = null;
            }
        }
    }
}
