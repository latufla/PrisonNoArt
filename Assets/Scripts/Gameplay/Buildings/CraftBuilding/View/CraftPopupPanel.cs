using Honeylab.Ads;
using Honeylab.Consumables;
using Honeylab.Consumables.Configs;
using Honeylab.Gameplay.Ui.Ads;
using Honeylab.Utils.Configs;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;


namespace Honeylab.Gameplay.Buildings
{
    public class CraftPopupPanel : MonoBehaviour
    {
        [SerializeField] private Transform _consumablePanelsRoot;
        [SerializeField] private TextMeshProUGUI _titleLabel;
        [SerializeField] private ConsumablesAmountPanel _resultPanel;
        [SerializeField] private Slider _amountSlider;
        [SerializeField] private Button _craftButtonEnabled;
        [SerializeField] private Transform _craftButtonDisabled;
        [SerializeField] private ConsumablesRewardedAdButton _consumablesRewardedAdButton;

        private List<ConsumablesAmountPanel> _consumablePanels;
        private CompositeDisposable _showIdleDisposable;
        private IDisposable _playerInputBlock;

        private ConsumablesData _consumablesData;
        private ConsumablesService _consumablesService;


        public void ShowIdle(CraftBuildingFlow flow)
        {
            _consumablesData ??= flow.Resolve<ConsumablesData>();
            _consumablesService ??= flow.Resolve<ConsumablesService>();

            _showIdleDisposable?.Dispose();
            _showIdleDisposable = null;

            UpdateCraftAmountSlider(flow);
            UpdateConsumablePanels(flow);

            IConfigsService configs = flow.Resolve<IConfigsService>();
            RewardedAdsConsumablesConfig config = configs.Get<RewardedAdsConsumablesConfig>(flow.ConfigId);
            RewardedAdsService rewardedAdsService = flow.Resolve<RewardedAdsService>();

            _consumablesRewardedAdButton.Init(config.RewardedAdConsumables,
                _consumablesData,
                _consumablesService,
                rewardedAdsService,
                flow.Id,
                RewardedAdsLocation.GiveConsumablesCraft);
            _consumablesRewardedAdButton.Run();

            _showIdleDisposable = new();
            IDisposable craftAmountSliderChange = OnAmountSliderClickAsObserver()
                .Subscribe(amount =>
                {
                    flow.CraftAmount.Value = (int)amount;
                    UpdateConsumablePanels(flow);
                });
            _showIdleDisposable.Add(craftAmountSliderChange);
        }


        public void SetTitle(string title)
        {
            _titleLabel.gameObject.SetActive(!string.IsNullOrEmpty(title));
            _titleLabel.text = title;
        }


        public void Clear()
        {
            _consumablesRewardedAdButton.Clear();

            _playerInputBlock?.Dispose();
            _playerInputBlock = null;

            _showIdleDisposable?.Dispose();
            _showIdleDisposable = null;
        }


        private void UpdateCraftAmountSlider(CraftBuildingFlow flow)
        {
            var prices = flow.Config.CraftPrice;
            int possibleResultAmount = int.MaxValue;
            int n = prices.Count;
            for (int index = 0; index < n; ++index)
            {
                RewardAmountConfig price = prices[index];
                int resultAmount = GetResultAmount(price);
                if (resultAmount < possibleResultAmount)
                {
                    possibleResultAmount = resultAmount;
                }
            }

            _amountSlider.minValue = 0;
            if (possibleResultAmount == 0)
            {
                _amountSlider.interactable = false;
                _amountSlider.maxValue = 1;
                _amountSlider.value = 0;
            }
            else
            {
                _amountSlider.interactable = true;
                _amountSlider.maxValue = possibleResultAmount;
                _amountSlider.value = flow.CraftAmount.Value;
            }
        }


        private void UpdateConsumablePanels(CraftBuildingFlow flow)
        {
            var consumablePanels = GetConsumablePanels();
            int n = consumablePanels.Count;
            int m = flow.Config.CraftPrice.Count;
            bool hasEnoughConsumables = true;
            for (int i = 0; i < n; ++i)
            {
                ConsumablesAmountPanel panel = consumablePanels[i];
                bool hasPrice = i < m;
                panel.gameObject.SetActive(hasPrice);

                if (!hasPrice)
                {
                    continue;
                }

                RewardAmountConfig price = flow.Config.CraftPrice[i];
                ConsumableData data = _consumablesData.GetData(price.Name);
                panel.SetIcon(data.Sprite);

                int priceAmount = price.Amount * flow.CraftAmount.Value;
                var amountProp = _consumablesService.GetAmountProp(data.Id);
                panel.SetAmount(Mathf.Clamp(amountProp.Value, 0, priceAmount), priceAmount);

                if (hasEnoughConsumables && amountProp.Value < priceAmount)
                {
                    hasEnoughConsumables = false;
                }
            }

            SetCraftButtonEnabled(hasEnoughConsumables);

            RewardAmountConfig result = flow.Config.CraftResult;
            ConsumableData resultData = _consumablesData.GetData(result.Name);
            _resultPanel.SetIcon(resultData.Sprite);
            _resultPanel.SetAmount(result.Amount * flow.CraftAmount.Value);

            LayoutRebuilder.ForceRebuildLayoutImmediate(_consumablePanelsRoot as RectTransform);
            LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
            LayoutRebuilder.ForceRebuildLayoutImmediate(transform.parent as RectTransform);
        }


        private IObservable<float> OnAmountSliderClickAsObserver() => _amountSlider.onValueChanged.AsObservable();
        public IObservable<Unit> OnCraftButtonClickAsObserver() => _craftButtonEnabled.OnClickAsObservable();


        private void SetCraftButtonEnabled(bool isEnabled)
        {
            _craftButtonEnabled.gameObject.SetActive(isEnabled);
            _craftButtonDisabled.gameObject.SetActive(!isEnabled);
        }


        private List<ConsumablesAmountPanel> GetConsumablePanels()
        {
            _consumablePanels ??=
                _consumablePanelsRoot.GetComponentsInChildren<ConsumablesAmountPanel>(true).ToList();
            return _consumablePanels;
        }


        private int GetResultAmount(RewardAmountConfig price)
        {
            ConsumableData data = _consumablesData.GetData(price.Name);
            var amountProp = _consumablesService.GetAmountProp(data.Id);

            return amountProp.Value / price.Amount;
        }
    }
}
