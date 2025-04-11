namespace Honeylab.Utils.Ads
{
    public readonly struct RewardedVideoDismissedEventArgs : IPlacementAccess, IClickedStateAccess, INetworkNameAccess
    {
        public readonly IRewardedVideoService Service;
        public readonly RewardedVideoShowResult ShowResult;
        private readonly string _networkName;


        public RewardedVideoDismissedEventArgs(IRewardedVideoService service,
            string placement,
            RewardedVideoShowResult showResult) : this(service, placement, showResult, null, null) { }


        private RewardedVideoDismissedEventArgs(IRewardedVideoService service,
            string placement,
            RewardedVideoShowResult showResult,
            bool? isClicked,
            string networkName)
        {
            Service = service;
            Placement = placement;
            ShowResult = showResult;
            IsClicked = isClicked;
            _networkName = networkName;
        }


        public string Placement { get; }
        public bool? IsClicked { get; }


        public bool TryGetNetworkName(out string networkName)
        {
            networkName = _networkName;
            return !string.IsNullOrEmpty(networkName);
        }


        public RewardedVideoDismissedEventArgs WithClickStatus(bool isClicked) =>
            new RewardedVideoDismissedEventArgs(Service, Placement, ShowResult, isClicked, _networkName);


        public RewardedVideoDismissedEventArgs WithNetworkName(string networkName) =>
            new RewardedVideoDismissedEventArgs(Service, Placement, ShowResult, IsClicked, networkName);


        public override string ToString() =>
            $"{nameof(ShowResult)}: {ShowResult}, {nameof(_networkName)}: {(string.IsNullOrEmpty(_networkName) ? "N/A" : _networkName)}, {nameof(Placement)}: {Placement}, {nameof(IsClicked)}: {IsClicked}";
    }
}
