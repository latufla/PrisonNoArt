using Honeylab.Ads;
using Honeylab.Gameplay.Ui.Ads;
using Honeylab.Gameplay.Ui.Consumables;
using Honeylab.Utils;
using Honeylab.Utils.Data;
using Honeylab.Utils.Formatting;
using System;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;


namespace Honeylab.Gameplay.Ui
{
    public class ExtraConsumablesScreen : ScreenBase
    {
        [SerializeField] private Button _backgroundButton;
        [SerializeField] private AmountProgressPanel _amountProgressPanel;
        [SerializeField] private TextMeshProUGUI _rewardedAdAmountLabel;
        [SerializeField] private RewardedAdButton _rewardedAdButton;
        [SerializeField] private TextMeshProUGUI _consumablesAmountLabel;
        [SerializeField] private ConsumablesButton _consumablesAmountButton;
        [SerializeField] private List<Image> _icons;

        [SerializeField] private Transform _adRewardFlyStart;
        [SerializeField] private Transform _consumablesRewardFlyStart;

        private CompositeDisposable _runDisposable;

        private string _rewardedAdAmountLabelFormat;
        private string _consumablesAmountLabelFormat;

        public Vector3 AdRewardFlyStartPosition => _adRewardFlyStart.position;
        public Vector3 ConsumablesRewardFlyStartPosition => _consumablesRewardFlyStart.position;

        public override string Name => ScreenName.ExtraConsumables;
        public AmountProgressPanel AmountProgressPanel => _amountProgressPanel;
        public bool IsRewardedAdActive() => _rewardedAdButton.IsRewardedAdActive();



        public IObservable<RewardedAdResult> OnRewardedAdShownAsObservable() =>
            _rewardedAdButton.OnRewardedAdShownAsObservable();


        public ConsumablesButton ConsumablesAmountButton => _consumablesAmountButton;
        public List<Image> Icons => _icons;


        public void SetRewardedAdConsumablesAmountText(int amount)
        {
            _rewardedAdAmountLabelFormat ??= _rewardedAdAmountLabel.text;

            string amountText = AcronymedPrint.ToString(amount);
            _rewardedAdAmountLabel.text = string.Format(_rewardedAdAmountLabelFormat, amountText);
        }


        public void SetConsumablesAmountText(int amount)
        {
            _consumablesAmountLabelFormat ??= _consumablesAmountLabel.text;

            string amountText = AcronymedPrint.ToString(amount);
            _consumablesAmountLabel.text = string.Format(_consumablesAmountLabelFormat, amountText);
        }


        public override IObservable<Unit> OnCloseButtonClickAsObservable() => base.OnCloseButtonClickAsObservable()
            .Merge(_backgroundButton.OnClickAsObservable());


        public void Run(RewardedAdsService rewardedAdsService, ScriptableId id, string location)
        {
            _runDisposable?.Dispose();
            _runDisposable = new CompositeDisposable();

            _rewardedAdButton.Init(rewardedAdsService, id, location);
            _rewardedAdButton.Run();
        }


        public void Stop()
        {
            _rewardedAdButton.Clear();

            _runDisposable?.Dispose();
            _runDisposable = null;
        }
    }
}
