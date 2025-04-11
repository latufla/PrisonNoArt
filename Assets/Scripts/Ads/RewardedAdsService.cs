using Honeylab.Utils;
using Honeylab.Utils.Data;
using System;
using UniRx;


namespace Honeylab.Ads
{
    public enum RewardedAdResultState
    {
        Clicked,
        Success,
        Failed,
        Available
    }

    public struct RewardedAdResult
    {
        public RewardedAdResultState State { get; set; }
        public ScriptableId Id { get; set; }
        public string Location { get; set; }
    }


    public class RewardedAdsService : IDisposable
    {
        private readonly TimeService _timeService;

        private readonly ISubject<RewardedAdResult> _clickedSubject = new Subject<RewardedAdResult>();
        private readonly ISubject<RewardedAdResult> _showSubject = new Subject<RewardedAdResult>();
        private readonly ISubject<RewardedAdResult> _resultShownSubject = new Subject<RewardedAdResult>();
        private bool _isRewardedAdActive;


        public bool CanShowRewardedAd() => IsRewardedAdReady() && !IsRewardedAdActive();

        public IObservable<RewardedAdResult> OnRewardedAdClickedAsObservable() => _clickedSubject.AsObservable();
        public IObservable<RewardedAdResult> OnRewardedAdStartShowAsObservable() => _showSubject.AsObservable();
        public IObservable<RewardedAdResult> OnRewardedAdResultShownAsObservable() => _resultShownSubject.AsObservable();


        public RewardedAdsService(TimeService timeService)
        {
            _timeService = timeService;
        }


        public void Dispose() { }


        public bool ShowRewardedAd(ScriptableId id, string location = RewardedAdsLocation.Common)
        {
            _clickedSubject.OnNext(new RewardedAdResult { State = RewardedAdResultState.Clicked, Id = id, Location = location });

            if (!CanShowRewardedAd())
            {
                _resultShownSubject.OnNext(new RewardedAdResult { State = RewardedAdResultState.Failed, Id = id, Location = location });
                return false;
            }

            _isRewardedAdActive = true;
            _timeService.Pause();

            _showSubject.OnNext(new RewardedAdResult { State = RewardedAdResultState.Available, Id = id, Location = location });
            return true;
        }


        private void OnRewardedAdResult(bool result, ScriptableId id, string location)
        {
            _isRewardedAdActive = false;
            _timeService.Resume();

            _resultShownSubject.OnNext(new RewardedAdResult { State = result == true ? RewardedAdResultState.Success : RewardedAdResultState.Failed, Id = id, Location = location });
        }


        public bool IsRewardedAdActive() => _isRewardedAdActive;
        private bool IsRewardedAdReady() => false;
    }
}
