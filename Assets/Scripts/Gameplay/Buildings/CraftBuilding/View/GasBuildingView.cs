using Cysharp.Threading.Tasks;
using Honeylab.Ads;
using Honeylab.Consumables;
using Honeylab.Consumables.Configs;
using Honeylab.Gameplay.Analytics;
using Honeylab.Gameplay.Pools;
using Honeylab.Gameplay.SimpleGet;
using Honeylab.Gameplay.SpeedUp;
using Honeylab.Gameplay.Ui;
using Honeylab.Gameplay.Ui.SimpleGet;
using Honeylab.Gameplay.Ui.Speedup;
using Honeylab.Gameplay.World;
using Honeylab.Utils;
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
    public class GasBuildingView : CraftBuildingViewBase
    {
        [SerializeField] private GasBuildingPopupView _gasPopupView;
        [SerializeField] private ScriptableId _offscreenIndicatorId;

        private CraftBuildingFlow _flow;
        private ConsumablesService _consumables;

        private GasBuildingPopup _gasPopup;
        private OffscreenIndicator _offscreenIndicator;

        private CompositeDisposable _disposable;
        private CompositeDisposable _showPopupDisposable;
        private ToastsService _toastsService;
        private ConsumablesData _consumablesData;

        private CancellationTokenSource _screenCts;

        private TimeService _time;

        protected override void OnInit()
        {
            _flow = GetFlow<CraftBuildingFlow>();
            _consumables = _flow.Resolve<ConsumablesService>();
            _consumablesData = _flow.Resolve<ConsumablesData>();
            _time = _flow.Resolve<TimeService>();

            GameplayPoolsService pools = _flow.Resolve<GameplayPoolsService>();
            BillboardPresenterFactory billboards = _flow.Resolve<BillboardPresenterFactory>();

            _toastsService = _flow.Resolve<ToastsService>();
            _gasPopupView.Init(pools, billboards);

            _disposable = new CompositeDisposable();

            OffscreenIndicatorsService indicators = _flow.Resolve<OffscreenIndicatorsService>();
            IDisposable stateChange = _flow.State.ValueProperty
                .Subscribe(it =>
                {
                    if (it == CraftBuildingStates.Done)
                    {
                        ShowCraftPopup();
                        _offscreenIndicator = indicators.Add(_offscreenIndicatorId, transform);

                        RewardAmountConfig result = _flow.Config.CraftResult;
                        ConsumableData resultData = _consumablesData.GetData(result.Name);
                        _offscreenIndicator.SetIcon(resultData.Sprite);
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

            _disposable.Add(stateChange);

            if (_flow.State.Value == CraftBuildingStates.Idle)
            {
                StartCraft();
            }
        }

        protected override void OnClear()
        {
            _showPopupDisposable?.Dispose();
            _showPopupDisposable = null;

            _disposable?.Dispose();
            _disposable = null;

            base.OnClear();
        }

        public override bool IsCraftPopupShown()  => _gasPopup != null;


        public override void ShowCraftPopup()
        {
            if (_gasPopup != null)
            {
                return;
            }
            _gasPopup = _gasPopupView.Show();

            _showPopupDisposable = new CompositeDisposable();

            IDisposable collect = _gasPopup.CraftWorkPanel.OnCollectButtonClickAsObserver()
                .Subscribe(_ =>
                {
                    var amount = _flow.GetCurrentAmount();
                    _consumables.TryGiveAmount(_flow.Config.CraftResult, new TransactionSource(_flow.name, TransactionType.Craft), amount);

                    ShowToast(amount);

                    _flow.State.Value = CraftBuildingStates.Done;
                    double oldTime = _flow.TimeLeft.Value;
                    _flow.TimeLeft.Value = oldTime + amount * _flow.Config.CraftDuration;
                    _flow.CraftStartTime.Value = _time.GetUtcTime();
                    StartCraft();
                });
            _showPopupDisposable.Add(collect);

            UpdateCraftPopup(_flow.State.Value);

            IDisposable stateChange = _flow.State.ValueProperty.Pairwise()
                .Subscribe(it =>
                {
                    if (it is { Previous: CraftBuildingStates.Done, Current: CraftBuildingStates.Idle })
                    {
                        HideCraftPopup();
                    }
                    else
                    {
                        UpdateCraftPopup(it.Current);
                    }
                });
            _showPopupDisposable.Add(stateChange);

            IDisposable onGetConsumablesClick = _gasPopup.CraftWorkPanel.OnGetConsumablesButtonClickAsObserver()
                .Subscribe(_ =>
                {
                    GetConsuamablesClick(_flow, ScreenOpenType.RequiredClick);
                });
            _showPopupDisposable.Add(onGetConsumablesClick);
        }


        private void GetConsuamablesClick(ISimpleGetAgent agent, ScreenOpenType screenOpenType)
        {
            var presenter = new SimpleGetScreenPresenter(agent, RewardedAdsLocation.Get);

            _screenCts?.CancelThenDispose();
            _screenCts = new CancellationTokenSource();
            presenter.RunAsync(screenOpenType, _screenCts.Token).Forget();
        }

        private void StartCraft()
        {
            _flow.CraftAmount.Value = 10;
            _flow.State.Value = CraftBuildingStates.Work;
        }

        public override void HideCraftPopup()
        {
            _screenCts?.CancelThenDispose();
            _screenCts = null;

            _showPopupDisposable?.Dispose();
            _showPopupDisposable = null;

            if (_gasPopup != null)
            {
                _gasPopup.CraftWorkPanel.Clear();
                _gasPopupView.HideAsync(true).Forget();

                _gasPopup = null;
            }
        }


        public override void UpdateCraftPopup(CraftBuildingStates state)
        {
            _gasPopup.ShowWork(_flow);
        }


        private void ShowToast(int amount)
        {
            Vector3 toastPosition = transform.position + Vector3.up * 5.0f;
            ConsumableData data = _consumablesData.GetData(_flow.Config.CraftResult.Name);
            ConsumablePersistenceId rewardId = data.Id;
            _toastsService.ShowConsumableToast(new ConsumableToastArgs(rewardId,
                amount,
                toastPosition));
        }


        protected override List<Renderer> GetSkinRenderers() => new() { SkinRenderer };
    }
}
