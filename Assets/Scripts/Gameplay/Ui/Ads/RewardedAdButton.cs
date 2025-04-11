using Cysharp.Threading.Tasks;
using Honeylab.Ads;
using Honeylab.MoneyGarden.Ui;
using Honeylab.Utils.Data;
using Honeylab.Utils.Extensions;
using System;
using System.Threading;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;


namespace Honeylab.Gameplay.Ui.Ads
{
    public class RewardedAdButton : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private LoadingSpinnerView _spinner;
        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _label;

        private RewardedAdsService _rewardedAdsService;
        private ScriptableId _id;
        private string _location;

        private CancellationTokenSource _cts;
        private CompositeDisposable _disposables;
        public Button Button => _button;


        public bool IsRewardedAdActive() => _rewardedAdsService.IsRewardedAdActive();


        public IObservable<RewardedAdResult> OnRewardedAdShownAsObservable() => _rewardedAdsService
            .OnRewardedAdResultShownAsObservable()
            .Where(it => it.Id.Equals(_id));


        public void Init(RewardedAdsService rewardedAdsService, ScriptableId id, string location)
        {
            _rewardedAdsService = rewardedAdsService;
            _id = id;
            _location = location;
        }


        public void Run()
        {
            _cts?.CancelThenDispose();
            _cts = new CancellationTokenSource();
            _disposables = new CompositeDisposable();

            IDisposable onRewardShown = _rewardedAdsService.OnRewardedAdResultShownAsObservable()
                .Subscribe(_ =>
                {
                    Clear();
                    Run();
                });
            _disposables.Add(onRewardShown);

            RunAsync(_cts.Token).Forget();
        }


        private async UniTask RunAsync(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                await UniTask.Yield(ct);

                if (!_rewardedAdsService.CanShowRewardedAd())
                {
                    ShowAdNotReady();
                }

                await UniTask.WaitUntil(_rewardedAdsService.CanShowRewardedAd, cancellationToken: ct);
                ShowAdReady();

                await _button.OnClickAsObservable().ToUniTask(true, cancellationToken: ct);
                _rewardedAdsService.ShowRewardedAd(_id, _location);
            }
        }


        public void Clear()
        {
            OnClear();

            _cts?.CancelThenDispose();
            _cts = null;
            _disposables?.Dispose();
            _disposables = null;

            SpinnerActive(false);
        }


        protected virtual void OnClear() { }


        public void SetLabel(string label)
        {
            if (_label != null)
            {
                _label.text = label;
            }
        }


        public void SetIcon(Sprite sprite)
        {
            if (_icon != null)
            {
                _icon.sprite = sprite;
            }
        }


        private void ShowAdNotReady()
        {
            SpinnerActive(true);
        }


        private void ShowAdReady()
        {
            SpinnerActive(false);
        }


        private void SpinnerActive(bool active)
        {
            if (_spinner == null)
            {
                return;
            }

            _spinner.SetTweening(active);
            _spinner.gameObject.SetActive(active);

            _icon.gameObject.SetActive(!active);
        }


        public void SetActive(bool isEnabled)
        {
            gameObject.SetActive(isEnabled);
        }
    }
}
