using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Honeylab.Consumables;
using Honeylab.Gameplay.Analytics;
using Honeylab.Gameplay.Pools;
using Honeylab.Gameplay.Ui;
using Honeylab.Utils.Data;
using Honeylab.Utils.OffscreenTargetIndicators;
using UniRx;
using UnityEngine;

namespace Honeylab.Gameplay.Buildings
{
    public class CraftBuildingView : CraftBuildingViewBase
    {
        [SerializeField] private CraftBuildingPopupView _craftPopupView;
        [SerializeField] private ScriptableId _offscreenIndicatorId;

        private CraftBuildingFlow _flow;
        private ConsumablesService _consumables;

        private CraftBuildingPopup _craftPopup;
        private OffscreenIndicator _offscreenIndicator;

        private CompositeDisposable _disposable;
        private CompositeDisposable _showPopupDisposable;
        private ToastsService _toastsService;
        private ConsumablesData _consumablesData;


        protected override void OnInit()
        {
            _flow = GetFlow<CraftBuildingFlow>();
            _consumables = _flow.Resolve<ConsumablesService>();
            _consumablesData = _flow.Resolve<ConsumablesData>();

            GameplayPoolsService pools = _flow.Resolve<GameplayPoolsService>();
            BillboardPresenterFactory billboards = _flow.Resolve<BillboardPresenterFactory>();

            _toastsService = _flow.Resolve<ToastsService>();
            _craftPopupView.Init(pools, billboards);

            _disposable = new CompositeDisposable();

            OffscreenIndicatorsService indicators = _flow.Resolve<OffscreenIndicatorsService>();
            IDisposable stateChange = _flow.State.ValueProperty
                .Subscribe(it =>
                {
                    if (it == CraftBuildingStates.Done)
                    {
                        ShowCraftPopup();

                        _offscreenIndicator = indicators.Add(_offscreenIndicatorId, transform);

                        ConsumableData data = _consumablesData.GetData(_flow.Config.CraftResult.Name);
                        _offscreenIndicator.SetIcon(data.Sprite);
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
        }


        protected override void OnClear()
        {
            _showPopupDisposable?.Dispose();
            _showPopupDisposable = null;

            _disposable?.Dispose();
            _disposable = null;

            base.OnClear();
        }


        public override bool IsCraftPopupShown() => _craftPopup != null;


        public override void ShowCraftPopup()
        {
            if (_craftPopup != null)
            {
                return;
            }

            _craftPopup = _craftPopupView.Show();

            _showPopupDisposable = new CompositeDisposable();
            IDisposable craft = _craftPopup.CraftPanel.OnCraftButtonClickAsObserver()
                .Subscribe(_ =>
                {
                    _consumables.WithdrawAmount(_flow.Config.CraftPrice,
                        new TransactionSource(_flow.name, TransactionType.Craft),
                        _flow.CraftAmount.Value);
                    _flow.State.Value = CraftBuildingStates.Work;
                });
            _showPopupDisposable.Add(craft);

            IDisposable collect = _craftPopup.CraftWorkPanel.OnCollectButtonClickAsObserver()
                .Subscribe(_ =>
                {
                    var amount = _flow.CraftAmount.Value;
                    _consumables.TryGiveAmount(_flow.Config.CraftResult,
                        new TransactionSource(_flow.name, TransactionType.Craft),
                        amount);

                    _flow.CraftAmount.Value = 1;
                    _flow.State.Value = CraftBuildingStates.Idle;

                    Vector3 toastPosition = transform.position + Vector3.up * 5.0f;
                    ConsumableData data = _consumablesData.GetData(_flow.Config.CraftResult.Name);
                    ConsumablePersistenceId rewardId = data.Id;
                    _toastsService.ShowConsumableToast(new ConsumableToastArgs(rewardId,
                        amount,
                        toastPosition));
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
        }


        public override void HideCraftPopup()
        {
            _showPopupDisposable?.Dispose();
            _showPopupDisposable = null;

            if (_craftPopup != null)
            {
                _craftPopup.CraftPanel.Clear();
                _craftPopup.CraftWorkPanel.Clear();
                _craftPopupView.HideAsync(true).Forget();

                _craftPopup = null;
            }
        }


        public override void UpdateCraftPopup(CraftBuildingStates state)
        {
            bool isIdle = state == CraftBuildingStates.Idle;
            _craftPopup.CraftPanel.gameObject.SetActive(isIdle);
            _craftPopup.CraftWorkPanel.gameObject.SetActive(!isIdle);

            ConsumableData data = _consumablesData.GetData(_flow.Config.CraftResult.Name);

            if (isIdle)
            {
                _craftPopup.CraftPanel.SetTitle(data.Title);
            }

            switch (state)
            {
                case CraftBuildingStates.Idle:
                    _craftPopup.CraftPanel.ShowIdle(_flow);
                    break;

                case CraftBuildingStates.Work:
                    _craftPopup.CraftPanel.Clear();
                    _craftPopup.CraftWorkPanel.SetTitle(data.Title);
                    _craftPopup.CraftWorkPanel.ShowWork(_flow);
                    break;

                case CraftBuildingStates.Done:
                    _craftPopup.CraftWorkPanel.SetTitle(data.Title);
                    _craftPopup.CraftWorkPanel.ShowWorkDone(_flow);
                    break;
            }
        }


        protected override List<Renderer> GetSkinRenderers() => new() { SkinRenderer };
    }
}