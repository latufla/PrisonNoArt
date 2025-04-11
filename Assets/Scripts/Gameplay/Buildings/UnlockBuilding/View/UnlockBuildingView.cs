using Cysharp.Threading.Tasks;
using Honeylab.Consumables;
using Honeylab.Consumables.Configs;
using Honeylab.Gameplay.Analytics;
using Honeylab.Gameplay.Pools;
using Honeylab.Gameplay.Ui;
using Honeylab.Persistence;
using Honeylab.Utils.Data;
using Honeylab.Utils.OffscreenTargetIndicators;
using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Honeylab.Gameplay.Buildings.View
{
    public class UnlockBuildingView : UnlockBuildingViewBase
    {
        [SerializeField] private string _titleText;
        [SerializeField] private Sprite _resultIconSprite;
        [SerializeField] private UnlockBuildingPopupView _unlockPopupView;

        [SerializeField] private ScriptableId _offscreenIndicatorId;


        private ConsumablesService _consumablesService;
        private ConsumablesData _consumablesData;
        private UnlockBuildingConsumablesPersistentComponent _consumablesPersistence;

        private UnlockBuildingPopup _unlockPopup;
        private OffscreenIndicator _offscreenIndicator;

        private CompositeDisposable _disposable;
        private CompositeDisposable _showPopupDisposable;


        public Sprite ResultIconSprite => _resultIconSprite;


        protected override void OnInit()
        {
            base.OnInit();
            _consumablesService = Flow.Resolve<ConsumablesService>();
            _consumablesData = Flow.Resolve<ConsumablesData>();

            GameplayPoolsService pools = Flow.Resolve<GameplayPoolsService>();
            BillboardPresenterFactory billboards = Flow.Resolve<BillboardPresenterFactory>();
            _unlockPopupView.Init(pools, billboards);

            _disposable = new CompositeDisposable();

            OffscreenIndicatorsService indicators = Flow.Resolve<OffscreenIndicatorsService>();
            IDisposable stateChange = Flow.State.ValueProperty
                .Subscribe(it =>
                {
                    if (it == UnlockBuildingStates.Claim)
                    {
                        ShowUnlockPopup();
                        if (_offscreenIndicatorId != null)
                        {
                            _offscreenIndicator = indicators.Add(_offscreenIndicatorId, transform);
                            _offscreenIndicator.SetIcon(_resultIconSprite);
                        }
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


        public override void ShowUnlockPopup()
        {
            if (_unlockPopup != null)
            {
                return;
            }

            _unlockPopup = _unlockPopupView.Show();

            _showPopupDisposable = new CompositeDisposable();
            IDisposable useButton = _unlockPopup.UnlockConsumablesPanel.OnUseButtonClickAsObservable()
                .Subscribe(index =>
                {
                    RewardAmountConfig config = Flow.Config.UnlockPrice[index];
                    ConsumableData data = _consumablesData.GetData(config.Name);
                    var amountProp = _consumablesService.GetAmountProp(data.Id);

                    int consumableAmount = Flow.Consumables.Amounts[index];
                    int needAmount = config.Amount - consumableAmount;
                    if (needAmount > 0)
                    {
                        int amountToWithdraw = Mathf.Clamp(amountProp.Value, 0, needAmount);
                        _consumablesService.WithdrawAmount(config.Name, amountToWithdraw, new TransactionSource(Flow.name, TransactionType.Unlock));
                        Flow.Consumables.Amounts[index] += amountToWithdraw;

                        UpdateUnlockPopup(Flow.State.Value);
                    }

                    if (Flow.IsSingleConsumableUnlock())
                    {
                        consumableAmount = Flow.Consumables.Amounts[index];
                        needAmount = config.Amount - consumableAmount;
                        if (needAmount > 0)
                        {
                            return;
                        }

                        if (Flow.IsConsumablesProgressUnlock())
                        {
                            Flow.State.Value = UnlockBuildingStates.Progress;
                        }
                        else
                        {
                            Flow.State.Value = UnlockBuildingStates.Unlocked;
                        }
                    }
                });
            _showPopupDisposable.Add(useButton);

            IDisposable emptyCompleteButton = _unlockPopup.OnEmptyCompleteButtonClickAsObservable()
                .Subscribe(_ => { Flow.State.Value = UnlockBuildingStates.Unlocked; });
            _showPopupDisposable.Add(emptyCompleteButton);

            IDisposable completeUseButton = _unlockPopup.UnlockConsumablesPanel.OnCompleteUseButtonClickAsObservable()
                .Subscribe(_ =>
                {
                    if (Flow.IsConsumablesProgressUnlock())
                    {
                        Flow.State.Value = UnlockBuildingStates.Progress;
                    }
                    else
                    {
                        Flow.State.Value = UnlockBuildingStates.Unlocked;
                    }
                });
            _showPopupDisposable.Add(completeUseButton);

            IDisposable claimUnlockButton = _unlockPopup.UnlockClaimPanel
                .OnClaimUnlockButtonClickAsObservable()
                .Subscribe(_ => { Flow.State.Value = UnlockBuildingStates.Unlocked; });
            _showPopupDisposable.Add(claimUnlockButton);

            IDisposable stateChange = Flow.State.ValueProperty.Subscribe(UpdateUnlockPopup);
            _showPopupDisposable.Add(stateChange);
        }


        public override void HideUnlockPopup()
        {
            _showPopupDisposable?.Dispose();
            _showPopupDisposable = null;

            if (_unlockPopup != null)
            {
                _unlockPopupView.HideAsync(true).Forget();
                _unlockPopup = null;
            }
        }


        public override void UpdateUnlockPopup(UnlockBuildingStates states)
        {
            _unlockPopup.UnlockConsumablesPanel.Hide();
            _unlockPopup.UnlockProgressPanel.Hide();
            _unlockPopup.UnlockClaimPanel.Hide();
            _unlockPopup.EmptyCompleteButton.gameObject.SetActive(false);

            _unlockPopup.SetTitle(_titleText);

            switch (states)
            {
                case UnlockBuildingStates.Idle:
                    if (Flow.IsConsumablesUnlock())
                    {
                        _unlockPopup.UnlockConsumablesPanel.Show(Flow);
                    }
                    else
                    {
                        _unlockPopup.EmptyCompleteButton.gameObject.SetActive(true);
                    }

                    break;

                case UnlockBuildingStates.Progress:
                    _unlockPopup.UnlockProgressPanel.Show(Flow, this);

                    break;

                case UnlockBuildingStates.Claim:
                    _unlockPopup.UnlockClaimPanel.Show(Flow, this);

                    break;

                case UnlockBuildingStates.Unlocked:
                    HideUnlockPopup();

                    break;
            }

            if (_unlockPopup != null)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(_unlockPopup.Layout);
                LayoutRebuilder.ForceRebuildLayoutImmediate(_unlockPopup.Layout);
            }
        }


        public override bool IsUnlockPopupShown() => _unlockPopup != null;


        protected override List<Renderer> GetSkinRenderers() => new() { SkinRenderer };
    }
}
