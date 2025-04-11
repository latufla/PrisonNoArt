using Honeylab.Ads;
using Honeylab.Consumables;
using Honeylab.Gameplay.Analytics;
using Honeylab.Gameplay.Player;
using Honeylab.Gameplay.SpeedUp;
using Honeylab.Project.Levels;
using Honeylab.Utils.Configs;
using System;
using System.Threading;
using UniRx;
using UnityEngine;


namespace Honeylab.Gameplay.Ui.Speedup
{
    public class SpeedUpScreenPresenter : ScreenPresenterBase<SpeedUpScreen>
    {
        private readonly ISpeedUpAgent _agent;
        private readonly PlayerInputService _playerInputService;
        private readonly RewardedAdsService _rewardedAdsService;
        private readonly ConsumablesData _consumablesData;
        private readonly ConsumablesService _consumablesService;
        private readonly IConfigsService _configs;
        private readonly LevelsLoadService _levelsLoadService;

        private readonly string _location;

        private CompositeDisposable _disposable;
        private IDisposable _blockInput;
        private SpeedUpConfig _config;


        public SpeedUpScreenPresenter(ISpeedUpAgent agent, string location) : base(
            agent.Resolve<ScreenFactory>())
        {
            _agent = agent;
            _location = location;

            _configs = agent.Resolve<IConfigsService>();
            _playerInputService = agent.Resolve<PlayerInputService>();
            _rewardedAdsService = agent.Resolve<RewardedAdsService>();
            _levelsLoadService = agent.Resolve<LevelsLoadService>();
            _consumablesData = agent.Resolve<ConsumablesData>();
            _consumablesService = agent.Resolve<ConsumablesService>();
        }


        protected override void OnRun(CancellationToken ct)
        {
            _blockInput = _playerInputService.BlockInput();

            LevelData levelData = _levelsLoadService.GetActiveLevelData();

            _config = _configs.Get<SpeedUpConfig>(levelData.SpeedUpConfigId);
            Screen.Run(_rewardedAdsService, _agent.Id, _location);

            _disposable?.Dispose();
            _disposable = new CompositeDisposable();

            string consumableName = _config.ConsumablesSpeedUpAmount.Name;
            ConsumableData consumable = _consumablesData.GetData(consumableName);
            Screen.ConsumablesSpeedUpButton.SetIcon(consumable.Sprite);

            Screen.RewardedAdSpeedUpTimePanel.SetTime(_config.RewardedAdSpeedUpTime);
            IDisposable timeLeftChanged = _agent.TimeLeft.Subscribe(timeLeft =>
            {
                if (_agent.TimeLeft.Value < 0.0 && !Screen.IsRewardedAdActive())
                {
                    Stop();
                    return;
                }

                Screen.TimeProgressPanel.SetTime((float)timeLeft, _agent.Duration);

                Screen.ConsumablesSpeedUpTimePanel.SetTime((float)timeLeft);
                int amount = CalcSpeedUpConsumablesAmount((float)timeLeft);
                bool hasEnoughAmount = _consumablesService.HasEnoughAmount(consumableName, amount);
                Screen.ConsumablesSpeedUpButton.SetEnabled(hasEnoughAmount);
                Screen.ConsumablesSpeedUpButton.SetAmount(amount);
            });
            _disposable.Add(timeLeftChanged);

            IDisposable rewardedAdShown = Screen.OnRewardedAdShownAsObservable()
                .Where(result => result.Id.Equals(_agent.Id))
                .Subscribe(result =>
                {
                    if (result.State == RewardedAdResultState.Success)
                    {
                        RewardedAdSpeedUp();
                    }

                    if (_agent.TimeLeft.Value < 0.0)
                    {
                        Stop();
                    }
                });
            _disposable.Add(rewardedAdShown);

            IDisposable consumablesButtonClick =
                Screen.ConsumablesSpeedUpButton.OnClickEnabledAsObservable()
                    .Subscribe(_ =>
                    {
                        int amount = CalcSpeedUpConsumablesAmount((float)_agent.TimeLeft.Value);
                        if (!_consumablesService.HasEnoughAmount(consumableName, amount))
                        {
                            return;
                        }

                        _consumablesService.WithdrawAmount(consumableName, amount, new TransactionSource(TransactionName.Skip, TransactionType.Skip));
                        ConsumablesSpeedUp();

                        if (_agent.TimeLeft.Value < 0.0)
                        {
                            Stop();
                        }
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


        private void RewardedAdSpeedUp()
        {
            _agent.SpeedUp(_config.RewardedAdSpeedUpTime);
        }


        private void ConsumablesSpeedUp()
        {
            _agent.SpeedUp((float)_agent.TimeLeft.Value);
        }


        private int CalcSpeedUpConsumablesAmount(float timeLeft) => Mathf.CeilToInt(
            _config.ConsumablesSpeedUpAmount.Amount *
            timeLeft / _config.ConsumablesSpeedUpTimePerAmount);
    }
}
