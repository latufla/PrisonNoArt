using Cysharp.Threading.Tasks;
using Honeylab.Ads;
using Honeylab.Consumables;
using Honeylab.Consumables.Configs;
using Honeylab.Gameplay.Analytics;
using Honeylab.Gameplay.Buildings;
using Honeylab.Gameplay.Player;
using Honeylab.Pools;
using Honeylab.Utils;
using Honeylab.Utils.Data;
using System;
using System.Linq;
using System.Threading;
using UniRx;
using UnityEngine;


namespace Honeylab.Gameplay.Ui
{
    public class ExtraConsumablesScreenPresenter : ScreenPresenterBase<ExtraConsumablesScreen>
    {
        private readonly IExtraConsumablesAgent _agent;
        private readonly string _location;

        private readonly PlayerInputService _playerInputService;
        private readonly RewardedAdsService _rewardedAdsService;
        private readonly ConsumablesData _consumablesData;
        private readonly ConsumablesService _consumablesService;
        private readonly GameplayScreenPresenter _gameplayScreenPresenter;

        private CompositeDisposable _disposable;
        private IDisposable _blockInput;

        private RewardAmountConfig _priceConfig;
        private RewardAmountConfig _rewardedAdConsumableAmountConfig;
        private int _extraConsumableAmountNeed;
        private int _extraConsumableNeedPrice;
        private ExtraConsumablePriceConfig _extraConsumablePrice;


        public ExtraConsumablesScreenPresenter(IExtraConsumablesAgent agent, string location) : base(
            agent.Resolve<ScreenFactory>())
        {
            _agent = agent;
            _location = location;

            _playerInputService = agent.Resolve<PlayerInputService>();
            _rewardedAdsService = agent.Resolve<RewardedAdsService>();
            _consumablesData = agent.Resolve<ConsumablesData>();
            _consumablesService = agent.Resolve<ConsumablesService>();
            _gameplayScreenPresenter = agent.Resolve<GameplayScreenPresenter>();
        }


        protected override void OnRun(CancellationToken ct)
        {
            _blockInput = _playerInputService.BlockInput();
            Screen.Run(_rewardedAdsService, _agent.Id, _location);

            UpdateScreen();

            if (Screen == null)
            {
                return;
            }

            _disposable?.Dispose();
            _disposable = new CompositeDisposable();

            IDisposable rewardedAdShown = Screen.OnRewardedAdShownAsObservable()
                .Subscribe(result =>
                {
                    if (result.State == RewardedAdResultState.Success)
                    {
                        //ConsumableData data = _consumablesData.GetData(_rewardedAdConsumableAmountConfig.Name);
                        //_gameplayScreenPresenter.ShowConsumableFlyToast(data, Screen.AdRewardFlyStartPosition, Screen.AdRewardFlyStartPosition);

                        _consumablesService.TryGiveAmount(_rewardedAdConsumableAmountConfig, new TransactionSource(TransactionName.RewardedAd, TransactionType.RV));
                        UpdateScreen();
                    }
                });
            _disposable.Add(rewardedAdShown);

            IDisposable consumablesButtonClick = Screen.ConsumablesAmountButton.OnClickEnabledAsObservable()
                .Subscribe(_ =>
                {
                    if (!_consumablesService.HasEnoughAmount(_extraConsumablePrice.Price.Name,
                            _extraConsumableNeedPrice))
                    {
                        return;
                    }
                    //ConsumableData data = _consumablesData.GetData(_extraConsumablePrice.Consumable.Name);
                    //_gameplayScreenPresenter.ShowConsumableFlyToast(data, Screen.ConsumablesRewardFlyStartPosition, Screen.ConsumablesRewardFlyStartPosition);

                    _consumablesService.WithdrawAmount(_extraConsumablePrice.Price.Name,
                        _extraConsumableNeedPrice,
                        new TransactionSource(TransactionName.RewardedAd, TransactionType.RV));

                    _consumablesService.TryGiveAmount(_extraConsumablePrice.Consumable.Name,
                        _extraConsumableAmountNeed,
                        new TransactionSource(TransactionName.RewardedAd, TransactionType.RV));

                    UpdateScreen();
                });
            _disposable.Add(consumablesButtonClick);
        }


        protected override void OnStop()
        {
            Screen.Stop();

            _blockInput?.Dispose();
            _blockInput = null;

            _disposable?.Dispose();
            _disposable = null;
        }


        private RewardAmountConfig GetRequiredPriceConfig() =>
            _agent.Price.FirstOrDefault(it => !_consumablesService.HasEnoughAmount(it));


        private void UpdateScreen()
        {
            _priceConfig = GetRequiredPriceConfig();
            if (_priceConfig == null)
            {
                Stop();
                return;
            }

            ConsumableData data = _consumablesData.GetData(_priceConfig.Name);
            var amountProp = _consumablesService.GetAmountProp(data.Id);
            Screen.AmountProgressPanel.SetAmount(amountProp.Value, _priceConfig.Amount);

            Screen.Icons.ForEach(it => it.sprite = data.Sprite);

            _rewardedAdConsumableAmountConfig =
                _agent.RewardedAdConsumablesAmountConfig.RewardedAdConsumablesAmounts.First(it =>
                    it.Name.Equals(_priceConfig.Name));

            Screen.SetRewardedAdConsumablesAmountText(_rewardedAdConsumableAmountConfig.Amount);

            _extraConsumablePrice =
                _agent.ExtraConsumablesPriceConfig.ConsumablePrices.First(it =>
                    it.Consumable.Name.Equals(_priceConfig.Name));

            ConsumableData extraConsumableData = _consumablesData.GetData(_extraConsumablePrice.Consumable.Name);
            var extraConsumableProp = _consumablesService.GetAmountProp(extraConsumableData.Id);
            _extraConsumableAmountNeed =
                _extraConsumablePrice.Consumable.Amount * _priceConfig.Amount - extraConsumableProp.Value;
            Screen.SetConsumablesAmountText(_extraConsumableAmountNeed);

            ConsumableData extraConsumablePriceData = _consumablesData.GetData(_extraConsumablePrice.Price.Name);
            Screen.ConsumablesAmountButton.SetIcon(extraConsumablePriceData.Sprite);

            _extraConsumableNeedPrice = _extraConsumablePrice.Price.Amount * _extraConsumableAmountNeed;
            bool hasEnoughAmount =
                _consumablesService.HasEnoughAmount(_extraConsumablePrice.Price.Name, _extraConsumableNeedPrice);
            Screen.ConsumablesAmountButton.SetEnabled(hasEnoughAmount);
            Screen.ConsumablesAmountButton.SetAmount(_extraConsumableNeedPrice);
        }
    }
}
