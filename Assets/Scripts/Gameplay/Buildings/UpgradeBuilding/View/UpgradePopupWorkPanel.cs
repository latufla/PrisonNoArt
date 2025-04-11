using Cysharp.Threading.Tasks;
using Honeylab.Ads;
using Honeylab.Gameplay.SpeedUp;
using Honeylab.Gameplay.Ui;
using Honeylab.Gameplay.Ui.Speedup;
using Honeylab.Gameplay.Weapons;
using Honeylab.Utils;
using Honeylab.Utils.Extensions;
using System;
using System.Threading;
using UniRx;
using UnityEngine;
using UnityEngine.UI;


namespace Honeylab.Gameplay.Buildings
{
    public class UpgradePopupWorkPanel : MonoBehaviour
    {
        [SerializeField] private TimeProgressPanel _progressPanel;
        [SerializeField] private UpgradeResultPanel _resultPanel;
        [SerializeField] private Button _collectButtonEnabled;
        [SerializeField] private Button _skipButton;

        private string _progressLabelFormat;
        private CompositeDisposable _showWorkDisposable;

        private SpeedUpScreenPresenter _speedUpScreenPresenter;
        private CancellationTokenSource _speedUpScreenCts;

        private WeaponsData _weaponsData;


        public IObservable<Unit> OnCollectButtonClickAsObserver() => _collectButtonEnabled.OnClickAsObservable();


        public void ShowWork(UpgradeBuildingFlow flow)
        {
            _weaponsData ??= flow.Resolve<WeaponsData>();

            _showWorkDisposable?.Dispose();
            _showWorkDisposable = null;

            int nextLevel = flow.WeaponUpgrade.GetNextLevel();
            WeaponData weaponData = _weaponsData.GetData(flow.WeaponId);
            WeaponLevelData weaponLevelData = weaponData.Levels[nextLevel - 1];
            _resultPanel.SetIcon(weaponLevelData.Sprite);
            _resultPanel.SetLevel(nextLevel);

            _showWorkDisposable = new();
            IDisposable updateProgress = flow.TimeLeft.Subscribe(timeLeft =>
            {
                _progressPanel.SetTime((float)timeLeft, flow.WeaponUpgradeConfig.Duration);
            });
            _showWorkDisposable.Add(updateProgress);

            _collectButtonEnabled.gameObject.SetActive(false);
            _progressPanel.gameObject.SetActive(true);

            _skipButton.gameObject.SetActive(!flow.SpeedUpConfig.SpeedUpHide);
            IDisposable skipClicked = _skipButton.OnClickAsObservable().Subscribe(_ => { ShowSpeedUpScreen(flow, ScreenOpenType.RequiredClick); });
            _showWorkDisposable.Add(skipClicked);

            LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
            LayoutRebuilder.ForceRebuildLayoutImmediate(transform.parent as RectTransform);
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


        public void ShowWorkDone(UpgradeBuildingFlow flow)
        {
            _weaponsData ??= flow.Resolve<WeaponsData>();

            _showWorkDisposable?.Dispose();
            _showWorkDisposable = null;

            int nextLevel = flow.WeaponUpgrade.GetNextLevel();
            WeaponData weaponData = _weaponsData.GetData(flow.WeaponId);
            WeaponLevelData weaponLevelData = weaponData.Levels[nextLevel - 1];
            _resultPanel.SetIcon(weaponLevelData.Sprite);
            _resultPanel.SetLevel(nextLevel);

            _progressPanel.gameObject.SetActive(false);
            _skipButton.gameObject.SetActive(false);

            UpgradeBuildingInteractable interactable = flow.Get<UpgradeBuildingInteractable>();
            bool isCollect = interactable != null && interactable.HasAgent();
            _collectButtonEnabled.gameObject.SetActive(isCollect);

            LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
            LayoutRebuilder.ForceRebuildLayoutImmediate(transform.parent as RectTransform);
        }


        private void ShowSpeedUpScreen(ISpeedUpAgent agent, ScreenOpenType screenOpenType)
        {
            _speedUpScreenPresenter = new SpeedUpScreenPresenter(agent, RewardedAdsLocation.SpeedUpUpgrade);

            _speedUpScreenCts?.CancelThenDispose();
            _speedUpScreenCts = new CancellationTokenSource();
            _speedUpScreenPresenter.RunAsync(screenOpenType, _speedUpScreenCts.Token).Forget();
        }
    }
}
