namespace Honeylab.Utils.Ads
{
    public readonly struct RewardedVideoClickedEventArgs : IPlacementAccess, INetworkNameAccess
    {
        public readonly IRewardedVideoService Service;
        private readonly string _networkName;


        public RewardedVideoClickedEventArgs(IRewardedVideoService service, string placement)
            : this(service, placement, null) { }


        private RewardedVideoClickedEventArgs(IRewardedVideoService service,
            string placement,
            string networkName)
        {
            Service = service;
            Placement = placement;
            _networkName = networkName;
        }


        public string Placement { get; }


        public bool TryGetNetworkName(out string networkName)
        {
            networkName = _networkName;
            return !string.IsNullOrEmpty(networkName);
        }


        public RewardedVideoClickedEventArgs WithNetworkName(string networkName) =>
            new RewardedVideoClickedEventArgs(Service, Placement, networkName);


        public override string ToString() =>
            $"{nameof(_networkName)}: {(string.IsNullOrEmpty(_networkName) ? "N/A" : _networkName)}, {nameof(Placement)}: {Placement}";
    }
}
