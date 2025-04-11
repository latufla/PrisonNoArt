using Honeylab.Ads;
using Honeylab.Consumables;
using Honeylab.Consumables.Configs;
using Honeylab.Gameplay.Analytics;
using Honeylab.Gameplay.Ui.Ads;
using Honeylab.Gameplay.World;
using Honeylab.Utils;
using Honeylab.Utils.Data;
using Honeylab.Utils.Pool;
using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;


namespace Honeylab.Gameplay.Ui.AdOffer
{
    public class AdOfferScreen : ScreenBase
    {
        [SerializeField] private RectTransform _root;
        [SerializeField] private Button _backgroundButton;
        [SerializeField] private GameObjectPoolBehaviour _pool;

        [SerializeField] private AdOfferScreenRewardPanel _rewardPanel;
        [SerializeField] private RewardedAdButton _rewardedAdButton;
        [SerializeField] private Image _rewardImage;
        [SerializeField] private Image _rewardSecondImage;
        [SerializeField] private TextMeshProUGUI _amountText;

        private CompositeDisposable _disposable;
        private CompositeDisposable _claimDisposable;
        private RewardedAdsService _rewardedAdsService;
        private ConsumablesData _consumablesData;
        private ConsumablesService _consumablesService;
        private TimeService _timeService;
        private ScriptableId _adOfferId;


        public override string Name => ScreenName.AdOffer;
        public override IObservable<Unit> OnCloseButtonClickAsObservable() => base.OnCloseButtonClickAsObservable()
            .Merge(_backgroundButton.OnClickAsObservable());


        public void Init(RewardedAdsService rewardedAdsService,
            ConsumablesData consumablesData,
            ConsumablesService consumablesService,
            WorldObjectId adOfferId,
            TimeService timeService)
        {
            _rewardedAdsService = rewardedAdsService;
            _consumablesData = consumablesData;
            _consumablesService = consumablesService;
            _timeService = timeService;
            _adOfferId = adOfferId;

            _rewardPanel.Init(_pool, _consumablesData);
            _rewardedAdButton.Init(_rewardedAdsService, _adOfferId, RewardedAdsLocation.AdOffer);
        }


        public void Run(RewardAmountConfig reward, Sprite rewardSprite, Action callback)
        {
            _disposable = new CompositeDisposable();
            LayoutRebuilder.ForceRebuildLayoutImmediate(_root);
            SetRewards(reward);
            _rewardImage.sprite = rewardSprite;
            _rewardedAdButton.Run();
            _backgroundButton.interactable = true;

            IDisposable onRewardButtonClick = _rewardedAdButton.Button.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    ScreenAction(ScreenParameters.Reward);
                    _backgroundButton.interactable = false;
                });
            _disposable.Add(onRewardButtonClick);

            IDisposable onRewardAdResult = _rewardedAdButton.OnRewardedAdShownAsObservable()
                .Subscribe(result =>
                {
                    if (result.State != RewardedAdResultState.Success)
                    {
                        CloseButton.onClick.Invoke();
                        return;
                    }

                    _rewardPanel.SetInfo(reward);
                    _timeService.Pause();
                    _claimDisposable?.Dispose();
                    _claimDisposable = new CompositeDisposable();

                    IDisposable onRewardClaim = _rewardPanel.OnClickButtonAsObservable()
                        .Subscribe(_ =>
                        {
                            _consumablesService.TryGiveAmount(reward, new TransactionSource(TransactionName.RewardedAd, TransactionType.RV));
                            callback.Invoke();
                            CloseButton.onClick.Invoke();
                        });
                    _claimDisposable.Add(onRewardClaim);
                });
            _disposable.Add(onRewardAdResult);

            LayoutRebuilder.ForceRebuildLayoutImmediate(_root);
        }


        private void SetRewards(RewardAmountConfig reward)
        {
            ConsumableData data = _consumablesData.GetData(reward.Name);
            _rewardSecondImage.sprite = data.Sprite;
            _amountText.text = reward.Amount.ToString();
        }


        public void Clear()
        {
            _disposable?.Dispose();
            _disposable = null;

            _claimDisposable?.Dispose();
            _claimDisposable = null;

            _rewardPanel.Clear();
            _rewardedAdButton.Clear();
        }
    }
}
