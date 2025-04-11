using Cysharp.Threading.Tasks;
using Honeylab.Ads;
using Honeylab.Consumables;
using Honeylab.Consumables.Configs;
using Honeylab.Gameplay.SpeedUp;
using Honeylab.Gameplay.Ui;
using Honeylab.Gameplay.Ui.Speedup;
using Honeylab.Utils;
using Honeylab.Utils.Extensions;
using System;
using System.Threading;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;


namespace Honeylab.Gameplay.Buildings
{
    public class CraftPopupWorkPanel : MonoBehaviour
    {
        [SerializeField] private TimeProgressPanel _progressPanel;
        [SerializeField] private ConsumablesAmountPanel _resultPanel;
        [SerializeField] private TMP_Text _titleLabel;
        [SerializeField] private Button _collectButtonEnabled;
        [SerializeField] private Button _skipButton;

        private string _progressLabelFormat;
        private CompositeDisposable _showWorkDisposable;
        private SpeedUpScreenPresenter _speedUpScreenPresenter;
        private CancellationTokenSource _speedUpScreenCts;


        public IObservable<Unit> OnCollectButtonClickAsObserver() => _collectButtonEnabled.OnClickAsObservable();


        public void ShowWork(CraftBuildingFlow flow)
        {
            _showWorkDisposable?.Dispose();
            _showWorkDisposable = null;

            ConsumablesData consumablesData = flow.Resolve<ConsumablesData>();

            RewardAmountConfig result = flow.Config.CraftResult;
            ConsumableData resultData = consumablesData.GetData(result.Name);
            _resultPanel.SetIcon(resultData.Sprite);
            _resultPanel.SetAmount(result.Amount * flow.CraftAmount.Value);

            _showWorkDisposable = new();
            IDisposable updateProgress = flow.TimeLeft.Subscribe(timeLeft =>
            {
                _progressPanel.SetTime((float)timeLeft, flow.Config.CraftDuration * flow.CraftAmount.Value);
            });
            _showWorkDisposable.Add(updateProgress);

            _progressPanel.gameObject.SetActive(true);
            _collectButtonEnabled.gameObject.SetActive(false);

            _skipButton.gameObject.SetActive(!flow.SpeedUpConfig.SpeedUpHide);
            IDisposable skipClicked = _skipButton.OnClickAsObservable().Subscribe(_ => { ShowSpeedUpScreen(flow, ScreenOpenType.RequiredClick); });
            _showWorkDisposable.Add(skipClicked);

            LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
        }


        private void ShowSpeedUpScreen(ISpeedUpAgent agent, ScreenOpenType screenOpenType)
        {
            _speedUpScreenPresenter = new SpeedUpScreenPresenter(agent, RewardedAdsLocation.SpeedUpCraft);

            _speedUpScreenCts?.CancelThenDispose();
            _speedUpScreenCts = new CancellationTokenSource();
            _speedUpScreenPresenter.RunAsync(screenOpenType, _speedUpScreenCts.Token).Forget();
        }


        public void Clear()
        {
            _progressPanel.gameObject.SetActive(false);
            _collectButtonEnabled.gameObject.SetActive(false);

            _skipButton.gameObject.SetActive(false);

            _speedUpScreenCts?.CancelThenDispose();
            _speedUpScreenCts = null;

            _showWorkDisposable?.Dispose();
            _showWorkDisposable = null;
        }


        public void ShowWorkDone(CraftBuildingFlow flow)
        {
            _showWorkDisposable?.Dispose();
            _showWorkDisposable = null;

            ConsumablesData consumablesData = flow.Resolve<ConsumablesData>();

            RewardAmountConfig result = flow.Config.CraftResult;
            ConsumableData resultData = consumablesData.GetData(result.Name);
            _resultPanel.SetIcon(resultData.Sprite);
            _resultPanel.SetAmount(result.Amount * flow.CraftAmount.Value);

            _progressPanel.gameObject.SetActive(false);
            _skipButton.gameObject.SetActive(false);

            CraftBuildingInteractable interactable = flow.Get<CraftBuildingInteractable>();
            bool isCollect = interactable != null && interactable.HasAgent();
            _collectButtonEnabled.gameObject.SetActive(isCollect);

            LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
        }


        public void SetTitle(string title)
        {
            _titleLabel.gameObject.SetActive(!string.IsNullOrEmpty(title));
            _titleLabel.text = title;
        }
    }
}
