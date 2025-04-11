using Honeylab.Ads;
using Honeylab.Consumables;
using Honeylab.Consumables.Configs;
using Honeylab.Gameplay.Analytics;
using Honeylab.Utils.Data;
using System;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;


namespace Honeylab.Gameplay.Ui.Ads
{
    public class ConsumablesRewardedAdButton : RewardedAdButton
    {
        [SerializeField] private Image _consumablesIcon;
        [SerializeField] private TextMeshProUGUI _consumablesAmountLabel;

        private string _consumablesAmountLabelFormat;

        private List<RewardAmountConfig> _rewards;
        private ConsumablesData _consumablesData;
        private RewardAmountConfig _reward;

        private CompositeDisposable _disposable;


        public void Init(List<RewardAmountConfig> rewards,
            ConsumablesData consumablesData,
            ConsumablesService consumablesService,
            RewardedAdsService rewardedAdsService,
            ScriptableId id,
            string location)
        {
            base.Init(rewardedAdsService, id, location);

            _rewards = rewards;
            _consumablesData = consumablesData;
            if (rewards == null || rewards.Count == 0)
            {
                gameObject.SetActive(false);
                return;
            }

            gameObject.SetActive(true);

            RefreshReward();

            _disposable = new CompositeDisposable();
            IDisposable rewardedAd = OnRewardedAdShownAsObservable()
                .Where(result => result.State == RewardedAdResultState.Success)
                .Subscribe(result =>
                {
                    if (consumablesService.TryGiveAmount(_reward, new TransactionSource(TransactionName.RewardedAd, TransactionType.RV)))
                    {
                        RefreshReward();
                    }
                });
            _disposable.Add(rewardedAd);
        }


        protected override void OnClear()
        {
            gameObject.SetActive(false);

            _disposable?.Dispose();
            _disposable = null;
        }


        private void RefreshReward()
        {
            if (_rewards == null)
                return;

            int index = Random.Range(0, _rewards.Count);
            _reward = _rewards[index];

            ConsumableData data = _consumablesData.GetData(_reward.Name);
            _consumablesIcon.sprite = data.Sprite;

            _consumablesAmountLabelFormat ??= _consumablesAmountLabel.text;
            _consumablesAmountLabel.text = string.Format(_consumablesAmountLabelFormat, _reward.Amount);
        }
    }
}
