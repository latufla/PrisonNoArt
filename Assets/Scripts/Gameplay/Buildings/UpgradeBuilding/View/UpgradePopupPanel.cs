using Cysharp.Threading.Tasks;
using Honeylab.Ads;
using Honeylab.Consumables;
using Honeylab.Consumables.Configs;
using Honeylab.Gameplay.Ui;
using Honeylab.Gameplay.Ui.Consumables;
using Honeylab.Gameplay.Weapons;
using Honeylab.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UniRx;
using UnityEngine;
using UnityEngine.UI;


namespace Honeylab.Gameplay.Buildings
{
    public class UpgradePopupPanel : MonoBehaviour
    {
        [SerializeField] private Transform _consumablePanelsRoot;
        [SerializeField] private UpgradeResultPanel _resultPanel;
        [SerializeField] private Button _upgradeButtonEnabled;
        [SerializeField] private Transform _upgradeButtonDisabled;
        [SerializeField] private ConsumablesButton _getConsumablesButton;

        private List<ConsumablesAmountPanel> _consumablePanels;

        private ConsumablesData _consumablesData;
        private ConsumablesService _consumablesService;
        private WeaponsData _weaponsData;

        private ExtraConsumablesScreenPresenter _extraConsumablesScreenPresenter;
        private CancellationTokenSource _extraConsumablesScreenCts;
        private CompositeDisposable _extraConsumablesScreenDisposable;
        private CompositeDisposable _disposable;


        public void Clear()
        {
            _getConsumablesButton.gameObject.SetActive(false);

            DisableUpgradeButtons();

            _extraConsumablesScreenCts?.CancelThenDispose();
            _extraConsumablesScreenCts = null;

            _extraConsumablesScreenDisposable?.Dispose();
            _extraConsumablesScreenDisposable = null;

            _disposable?.Dispose();
            _disposable = null;
        }


        public void ShowIdle(UpgradeBuildingFlow flow)
        {
            _consumablesData ??= flow.Resolve<ConsumablesData>();
            _consumablesService ??= flow.Resolve<ConsumablesService>();
            _weaponsData ??= flow.Resolve<WeaponsData>();

            _disposable?.Dispose();
            _disposable = new();

            UpdateConsumablePanels(flow);
            UpdateGetConsumablesButton(flow);
        }


        private void UpdateConsumablePanels(UpgradeBuildingFlow flow)
        {
            if (!flow.WeaponUpgrade.HasNextUpgrade())
            {
                DisableUpgradeButtons();
                SetConsumablePanelsEnabled(false);

                int maxLevel = flow.WeaponUpgrade.GetMaxLevel();
                WeaponData maxWeaponData = _weaponsData.GetData(flow.WeaponId);
                WeaponLevelData maxWeaponLevelData = maxWeaponData.Levels[maxLevel - 1];
                _resultPanel.SetIcon(maxWeaponLevelData.Sprite);
                _resultPanel.SetMaxLevel();

                return;
            }

            bool hasEnoughConsumables = flow.WeaponUpgrade.HasEnoughConsumables();
            SetUpgradeButtonEnabled(hasEnoughConsumables);
            SetConsumablePanelsEnabled(true);

            var consumablePanels = GetConsumablePanels();
            int n = consumablePanels.Count;
            int m = flow.WeaponUpgradeConfig.Price.Count;
            for (int i = 0; i < n; ++i)
            {
                ConsumablesAmountPanel panel = consumablePanels[i];
                bool hasPrice = i < m;
                panel.gameObject.SetActive(hasPrice);

                if (!hasPrice)
                {
                    continue;
                }

                RewardAmountConfig price = flow.WeaponUpgradeConfig.Price[i];
                ConsumableData data = _consumablesData.GetData(price.Name);
                panel.SetIcon(data.Sprite);

                int priceAmount = price.Amount;
                var amountProp = _consumablesService.GetAmountProp(data.Id);
                panel.SetAmount(Mathf.Clamp(amountProp.Value, 0, priceAmount), priceAmount);
            }

            int nextLevel = flow.WeaponUpgrade.GetNextLevel();
            WeaponData weaponData = _weaponsData.GetData(flow.WeaponId);
            WeaponLevelData weaponLevelData = weaponData.Levels[nextLevel - 1];
            _resultPanel.SetIcon(weaponLevelData.Sprite);
            _resultPanel.SetLevel(nextLevel);

            LayoutRebuilder.ForceRebuildLayoutImmediate(_consumablePanelsRoot as RectTransform);
        }


        private void UpdateGetConsumablesButton(UpgradeBuildingFlow flow)
        {
            if (flow.Price == null)
            {
                _getConsumablesButton.gameObject.SetActive(false);
                return;
            }

            RewardAmountConfig priceConfig = flow.Price.FirstOrDefault(it => !_consumablesService.HasEnoughAmount(it));
            bool showGetConsumables = priceConfig != null && flow.ExtraConsumablesPriceConfig.ConsumablePrices != null;
            _getConsumablesButton.gameObject.SetActive(showGetConsumables);
            if (showGetConsumables)
            {
                ConsumableData data = _consumablesData.GetData(priceConfig.Name);
                _getConsumablesButton.SetIcon(data.Sprite);
                _getConsumablesButton.SetAmount(priceConfig.Amount);
                _getConsumablesButton.SetEnabled(true);

                IDisposable getConsumables = _getConsumablesButton.OnClickEnabledAsObservable()
                    .Subscribe(_ =>
                    {
                        ShowExtraConsumablesScreen(flow, ScreenOpenType.RequiredClick);
                    });
                _disposable.Add(getConsumables);
            }
        }


        private void SetConsumablePanelsEnabled(bool isEnabled)
        {
            _consumablePanelsRoot.gameObject.SetActive(isEnabled);
        }


        public IObservable<Unit> OnUpgradeButtonClickAsObserver() => _upgradeButtonEnabled.OnClickAsObservable();


        private void SetUpgradeButtonEnabled(bool isEnabled)
        {
            _upgradeButtonEnabled.gameObject.SetActive(isEnabled);
            _upgradeButtonDisabled.gameObject.SetActive(!isEnabled);
        }


        private void DisableUpgradeButtons()
        {
            _upgradeButtonEnabled.gameObject.SetActive(false);
            _upgradeButtonDisabled.gameObject.SetActive(false);
        }


        private List<ConsumablesAmountPanel> GetConsumablePanels()
        {
            _consumablePanels ??=
                _consumablePanelsRoot.GetComponentsInChildren<ConsumablesAmountPanel>(true).ToList();
            return _consumablePanels;
        }


        private void ShowExtraConsumablesScreen(UpgradeBuildingFlow flow, ScreenOpenType screenOpenType)
        {
            _extraConsumablesScreenPresenter =
                new ExtraConsumablesScreenPresenter(flow, RewardedAdsLocation.GiveConsumablesUpgrade);

            _extraConsumablesScreenCts?.CancelThenDispose();
            _extraConsumablesScreenCts = new CancellationTokenSource();
            _extraConsumablesScreenPresenter.RunAsync(screenOpenType, _extraConsumablesScreenCts.Token).Forget();

            _extraConsumablesScreenDisposable?.Dispose();
            _extraConsumablesScreenDisposable = new CompositeDisposable();
            IDisposable stop = _extraConsumablesScreenPresenter.OnStopAsObservable().Subscribe(_ =>
            {
                UpdateConsumablePanels(flow);
                UpdateGetConsumablesButton(flow);
            });
            _extraConsumablesScreenDisposable.Add(stop);
        }
    }
}
