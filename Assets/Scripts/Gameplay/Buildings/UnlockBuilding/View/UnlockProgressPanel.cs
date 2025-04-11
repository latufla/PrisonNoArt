using Cysharp.Threading.Tasks;
using Honeylab.Ads;
using Honeylab.Gameplay.SpeedUp;
using Honeylab.Gameplay.Ui;
using Honeylab.Gameplay.Ui.Speedup;
using Honeylab.Utils;
using Honeylab.Utils.Extensions;
using System;
using System.Threading;
using UniRx;
using UnityEngine;
using UnityEngine.UI;


namespace Honeylab.Gameplay.Buildings.View
{
    public class UnlockProgressPanel : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private TimeProgressPanel _progressPanel;
        [SerializeField] private Button _skipButton;

        private SpeedUpScreenPresenter _speedUpScreenPresenter;
        private CancellationTokenSource _speedUpScreenCts;

        private CompositeDisposable _showDisposable;


        public void Show(UnlockBuildingFlow flow, UnlockBuildingView view)
        {
            _showDisposable?.Dispose();
            _showDisposable = new();

            gameObject.SetActive(true);
            _icon.sprite = view.ResultIconSprite;

            IDisposable updateProgress = flow.TimeLeft.Subscribe(timeLeft =>
            {
                _progressPanel.SetTime((float)timeLeft, flow.Config.UnlockDuration);
            });
            _showDisposable.Add(updateProgress);

            _skipButton.gameObject.SetActive(!flow.Config.SpeedUpHide);
            IDisposable skipClicked = _skipButton.OnClickAsObservable().Subscribe(_ => { ShowSpeedUpScreen(flow, ScreenOpenType.RequiredClick); });
            _showDisposable.Add(skipClicked);

            LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
        }


        public void Hide()
        {
            _skipButton.gameObject.SetActive(false);

            _speedUpScreenCts?.CancelThenDispose();
            _speedUpScreenCts = null;

            gameObject.SetActive(false);

            _showDisposable?.Dispose();
            _showDisposable = null;
        }


        private void ShowSpeedUpScreen(ISpeedUpAgent agent, ScreenOpenType screenOpenType)
        {
            _speedUpScreenPresenter = new SpeedUpScreenPresenter(agent, RewardedAdsLocation.SpeedUpFix);

            _speedUpScreenCts?.CancelThenDispose();
            _speedUpScreenCts = new CancellationTokenSource();
            _speedUpScreenPresenter.RunAsync(screenOpenType, _speedUpScreenCts.Token).Forget();
        }
    }
}
