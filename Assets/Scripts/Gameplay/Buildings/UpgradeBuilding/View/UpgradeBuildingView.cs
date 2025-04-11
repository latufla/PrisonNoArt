using Cysharp.Threading.Tasks;
using Honeylab.Consumables;
using Honeylab.Gameplay.Analytics;
using Honeylab.Gameplay.Pools;
using Honeylab.Gameplay.Ui;
using Honeylab.Gameplay.Weapons;
using Honeylab.Gameplay.World;
using Honeylab.Utils.Data;
using Honeylab.Utils.Extensions;
using Honeylab.Utils.OffscreenTargetIndicators;
using System;
using System.Collections.Generic;
using System.Threading;
using UniRx;
using UnityEngine;


namespace Honeylab.Gameplay.Buildings
{
    public class UpgradeBuildingView : WorldObjectComponentVisual
    {
        [SerializeField] private UpgradeBuildingPopupView _upgradePopupView;
        [SerializeField] private ScriptableId _offscreenIndicatorId;
        [SerializeField] private Renderer _skinRenderer;
        public Renderer SkinRenderer => _skinRenderer;

        private UpgradeBuildingFlow _flow;
        private ConsumablesService _consumables;

        private UpgradeBuildingPopup _upgradePopup;
        private OffscreenIndicator _offscreenIndicator;
        private GameplayScreen _gameplayScreen;

        private CompositeDisposable _disposable;
        private CompositeDisposable _showPopupDisposable;

        private CancellationTokenSource _cts;


        protected override void OnInit()
        {
            _flow = GetFlow<UpgradeBuildingFlow>();
            _consumables = _flow.Resolve<ConsumablesService>();
            _gameplayScreen = _flow.Resolve<GameplayScreen>();

            GameplayPoolsService pools = _flow.Resolve<GameplayPoolsService>();
            BillboardPresenterFactory billboards = _flow.Resolve<BillboardPresenterFactory>();
            _upgradePopupView.Init(pools, billboards);

            _disposable = new CompositeDisposable();
            OffscreenIndicatorsService indicators = _flow.Resolve<OffscreenIndicatorsService>();

            IDisposable stateChange = _flow.State.ValueProperty
                .Subscribe(it =>
                {
                    if (it == UpgradeBuildingStates.Done)
                    {
                        ShowUpgradePopup();

                        _offscreenIndicator = indicators.Add(_offscreenIndicatorId, transform);

                        WeaponLevelData nextWeaponLevelData = _flow.GetNextWeaponLevelData();
                        _offscreenIndicator.SetIcon(nextWeaponLevelData.Sprite);
                    }
                    else
                    {
                        if (_offscreenIndicator != null)
                        {
                            indicators.Remove(_offscreenIndicator);
                            _offscreenIndicator = null;
                        }
                    }
                });

            _gameplayScreen.WeaponUpgradeStatusPanel.SetBuilding(_flow);

            _disposable.Add(stateChange);
        }


        protected override void OnClear()
        {
            HideUpgradePopup();

            _disposable?.Dispose();
            _disposable = null;

            _cts?.CancelThenDispose();
            _cts = null;

            base.OnClear();
        }


        public bool IsUpgradePopupShown() => _upgradePopup != null;


        public void ShowUpgradePopup()
        {
            if (_upgradePopup != null)
            {
                return;
            }

            _upgradePopup = _upgradePopupView.Show();

            _showPopupDisposable = new CompositeDisposable();
            IDisposable upgrade = _upgradePopup.UpgradePanel.OnUpgradeButtonClickAsObserver()
                .Subscribe(_ =>
                {
                    if (_flow.WeaponUpgrade.CanUpgrade())
                    {
                        _consumables.WithdrawAmount(_flow.WeaponUpgradeConfig.Price,
                            new TransactionSource(_flow.name, TransactionType.Upgrade));
                        _flow.State.Value = UpgradeBuildingStates.Work;
                    }
                });
            _showPopupDisposable.Add(upgrade);

            IDisposable collect = _upgradePopup.UpgradeWorkPanel.OnCollectButtonClickAsObserver()
                .Subscribe(_ =>
                {
                    _flow.WeaponUpgrade.Upgrade();
                    _flow.State.Value = UpgradeBuildingStates.Idle;

                    HideUpgradePopup();
                });
            _showPopupDisposable.Add(collect);

            UpdateUpgradePopup(_flow.State.Value);

            IDisposable stateChange = _flow.State.ValueProperty.Pairwise()
                .Subscribe(it =>
                {
                    if (it is { Previous: UpgradeBuildingStates.Done, Current: UpgradeBuildingStates.Idle })
                    {
                        HideUpgradePopup();
                    }
                    else
                    {
                        UpdateUpgradePopup(it.Current);
                    }
                });
            _showPopupDisposable.Add(stateChange);
        }


        public void HideUpgradePopup()
        {
            _showPopupDisposable?.Dispose();
            _showPopupDisposable = null;

            if (_upgradePopup != null)
            {
                _upgradePopup.UpgradePanel.Clear();
                _upgradePopup.UpgradeWorkPanel.Clear();
                _upgradePopupView.HideAsync(true).Forget();

                _upgradePopup = null;
            }
        }


        public void UpdateUpgradePopup(UpgradeBuildingStates state)
        {
            bool isIdle = state == UpgradeBuildingStates.Idle;
            _upgradePopup.UpgradePanel.gameObject.SetActive(isIdle);
            _upgradePopup.UpgradeWorkPanel.gameObject.SetActive(!isIdle);

            switch (state)
            {
                case UpgradeBuildingStates.Idle:
                    _upgradePopup.UpgradePanel.ShowIdle(_flow);
                    break;

                case UpgradeBuildingStates.Work:
                    _upgradePopup.UpgradePanel.Clear();
                    _upgradePopup.UpgradeWorkPanel.ShowWork(_flow);
                    break;

                case UpgradeBuildingStates.Done:
                    _upgradePopup.UpgradeWorkPanel.ShowWorkDone(_flow);
                    break;
            }
        }


        protected override List<Renderer> GetSkinRenderers() => new() { SkinRenderer };
    }
}
