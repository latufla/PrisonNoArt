using Honeylab.Ads;
using Honeylab.Gameplay.Ui.Ads;
using Honeylab.Gameplay.Ui.Consumables;
using Honeylab.Utils;
using Honeylab.Utils.Data;
using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;


namespace Honeylab.Gameplay.Ui.Speedup
{
    public class SpeedUpScreen : ScreenBase
    {
        [SerializeField] private Button _backgroundButton;
        [SerializeField] private TimeProgressPanel _timeProgressPanel;
        [SerializeField] private TimePanel _rewardedAdSpeedUpTimePanel;
        [SerializeField] private RewardedAdButton _rewardedAdSpeedUpButton;
        [SerializeField] private TimePanel _consumablesSpeedUpTimePanel;
        [SerializeField] private ConsumablesButton _consumablesSpeedUpButton;


        private CompositeDisposable _runDisposable;


        public TimeProgressPanel TimeProgressPanel => _timeProgressPanel;
        public TimePanel RewardedAdSpeedUpTimePanel => _rewardedAdSpeedUpTimePanel;
        public bool IsRewardedAdActive() => _rewardedAdSpeedUpButton.IsRewardedAdActive();

        public override string Name => ScreenName.SpeedUp;
        public IObservable<RewardedAdResult> OnRewardedAdShownAsObservable() =>
            _rewardedAdSpeedUpButton.OnRewardedAdShownAsObservable();


        public TimePanel ConsumablesSpeedUpTimePanel => _consumablesSpeedUpTimePanel;
        public ConsumablesButton ConsumablesSpeedUpButton => _consumablesSpeedUpButton;


        public override IObservable<Unit> OnCloseButtonClickAsObservable() => base.OnCloseButtonClickAsObservable()
            .Merge(_backgroundButton.OnClickAsObservable());


        public void Run(RewardedAdsService rewardedAdsService, ScriptableId id, string location)
        {
            _runDisposable?.Dispose();
            _runDisposable = new CompositeDisposable();

            _rewardedAdSpeedUpButton.Init(rewardedAdsService, id, location);
            _rewardedAdSpeedUpButton.Run();
        }


        public void Stop()
        {
            _rewardedAdSpeedUpButton.Clear();

            _runDisposable?.Dispose();
            _runDisposable = null;
        }
    }
}
