using Cysharp.Threading.Tasks;
using Honeylab.Ads;
using Honeylab.Consumables;
using Honeylab.Gameplay.Analytics;
using Honeylab.Gameplay.Player;
using Honeylab.Gameplay.Pools;
using Honeylab.Gameplay.SimpleGet;
using Honeylab.Pools;
using Honeylab.Utils.Configs;
using Honeylab.Utils.Data;
using Honeylab.Utils.Extensions;
using System;
using System.Threading;
using UniRx;
using UnityEngine;


namespace Honeylab.Gameplay.Ui.SimpleGet
{
    public class SimpleGetScreenPresenter : ScreenPresenterBase<SimpleGetScreen>
    {
        private readonly ScriptableId _id;
        private readonly PlayerInputService _playerInputService;
        private readonly RewardedAdsService _rewardedAdsService;
        private readonly ConsumablesData _consumablesData;
        private readonly ConsumablesService _consumablesService;
        private readonly string _location;
        private readonly GameplayPoolsService _pools;
        private CompositeDisposable _disposable;
        private SimpleGetInfo _config;
        private CancellationTokenSource _cts;


        public SimpleGetScreenPresenter(ISimpleGetAgent agent,
            string location) : base(agent.Resolve<ScreenFactory>())
        {
            _location = location;
            IConfigsService configs = agent.Resolve<IConfigsService>();
            _playerInputService = agent.Resolve<PlayerInputService>();
            _rewardedAdsService = agent.Resolve<RewardedAdsService>();
            _consumablesData = agent.Resolve<ConsumablesData>();
            _consumablesService = agent.Resolve<ConsumablesService>();
            _pools = agent.Resolve<GameplayPoolsService>();
            _id = agent.Id;
            _config = configs.Get<SimpleGetConfig>(agent.ConfigId).SimpleGetInfo;
        }


        public SimpleGetScreenPresenter(ScreenFactory factory,
            PlayerInputService playerInputService,
            RewardedAdsService rewardedAdsService,
            ConsumablesData consumablesData,
            ConsumablesService consumablesService,
            string location,
            ScriptableId adRuntimeId,
            GameplayPoolsService pools) : base(factory)
        {
            _location = location;
            _playerInputService = playerInputService;
            _rewardedAdsService = rewardedAdsService;
            _consumablesData = consumablesData;
            _consumablesService = consumablesService;
            _id = adRuntimeId;
            _pools = pools;
        }


        protected override void OnRun(CancellationToken ct)
        {
            _disposable?.Dispose();
            _disposable = new CompositeDisposable();

            IDisposable blockInput = _playerInputService.BlockInput();
            _disposable.Add(blockInput);

            ItemFlyersPool itemFlyersPool = _pools.Get<ItemFlyersPool>();
            Screen.Init(itemFlyersPool);
            Screen.Run(_rewardedAdsService, _id, _location, _config.Title);
            StartWork(_config, _id);
        }


        private void StartWork(SimpleGetInfo config, ScriptableId id)
        {
            ConsumableData consumable = _consumablesData.GetData(config.ConsumablesRequired.Name);
            Screen.ConsumablesButton.SetIcon(consumable.Sprite);

            consumable = _consumablesData.GetData(config.RewardedAdResult.Name);
            Screen.SetInfoForRv(config.RewardedAdResult.Amount, consumable.Sprite);

            consumable = _consumablesData.GetData(config.ConsumablesResult.Name);
            ConsumableData consumableRequired = _consumablesData.GetData(config.ConsumablesRequired.Name);
            bool consumableRequiredHasEnough = _consumablesService.HasEnoughAmount(config.ConsumablesRequired.Name,
                config.ConsumablesRequired.Amount);
            Screen.SetInfoForConsumables(config.ConsumablesResult.Amount,
                config.ConsumablesRequired.Amount,
                consumable.Sprite,
                consumableRequired.Sprite,
                consumableRequiredHasEnough);

            _cts = new CancellationTokenSource();
            ConsumableData consumableResult = _consumablesData.GetData(config.ConsumablesResult.Name);
            var resultProp = _consumablesService.GetAmountProp(consumableResult.Id);
            Screen.SetConsumableCounterInfo(resultProp.Value, 0, consumableResult.Sprite, Vector3.zero, _cts.Token).Forget();

            IDisposable rewardedAdShown = Screen.OnRewardedAdShownAsObservable()
                .Where(result => result.Id.Equals(id))
                .Subscribe(result =>
                {
                    if (result.State == RewardedAdResultState.Success)
                    {
                        string consumableName = config.RewardedAdResult.Name;
                        int amount = config.RewardedAdResult.Amount;
                        _consumablesService.ChangeAmount(consumableName,
                            amount,
                            new TransactionSource(TransactionName.Get, TransactionType.RV));
                        Screen.SetConsumableCounterInfo(resultProp.Value, amount, consumableResult.Sprite, Screen.RewardedImagePosition, _cts.Token)
                            .Forget();
                    }
                });
            _disposable.Add(rewardedAdShown);

            IDisposable consumablesButtonClick =
                Screen.ConsumablesButton.OnClickEnabledAsObservable()
                    .Subscribe(_ =>
                    {
                        string consumableName = config.ConsumablesRequired.Name;
                        int amount = config.ConsumablesRequired.Amount;

                        string rewardConsumableName = config.ConsumablesResult.Name;
                        int rewardAmount = config.ConsumablesResult.Amount;
                        bool hasEnough = _consumablesService.HasEnoughAmount(consumableName, amount);
                        if (!hasEnough)
                        {
                            return;
                        }

                        _consumablesService.WithdrawAmount(consumableName,
                            amount,
                            new TransactionSource(TransactionName.Get, TransactionType.Get));
                        _consumablesService.ChangeAmount(rewardConsumableName,
                            rewardAmount,
                            new TransactionSource(TransactionName.Get, TransactionType.Get));

                        consumableRequired = _consumablesData.GetData(consumableName);
                        consumable = _consumablesData.GetData(rewardConsumableName);
                        hasEnough = _consumablesService.HasEnoughAmount(consumableName, amount);
                        Screen.SetInfoForConsumables(rewardAmount,
                            amount,
                            consumable.Sprite,
                            consumableRequired.Sprite,
                            hasEnough);
                        Screen.SetConsumableCounterInfo(resultProp.Value, rewardAmount, consumableResult.Sprite, Screen.ConsumablesImagePosition, _cts.Token)
                            .Forget();
                    });
            _disposable.Add(consumablesButtonClick);
        }


        protected override void OnStop()
        {
            Screen.Stop();

            _disposable?.Dispose();
            _disposable = null;

            _cts?.CancelThenDispose();
            _cts = null;

            _config = null;
        }
    }
}
