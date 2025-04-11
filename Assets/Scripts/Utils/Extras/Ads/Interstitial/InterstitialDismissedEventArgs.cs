namespace Honeylab.Utils.Ads
{
    public readonly struct InterstitialDismissedEventArgs : IPlacementAccess, IClickedStateAccess, INetworkNameAccess
    {
        public readonly IInterstitialService Service;
        private readonly string _networkName;


        public InterstitialDismissedEventArgs(IInterstitialService service, string placement)
            : this(service, placement, null, null) { }


        private InterstitialDismissedEventArgs(IInterstitialService service,
            string placement,
            bool? isClicked,
            string networkName)
        {
            Service = service;
            Placement = placement;
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


        public InterstitialDismissedEventArgs WithClickStatus(bool isClicked) =>
            new InterstitialDismissedEventArgs(Service, Placement, isClicked, _networkName);


        public InterstitialDismissedEventArgs WithNetworkName(string networkName) =>
            new InterstitialDismissedEventArgs(Service, Placement, IsClicked, networkName);


        public override string ToString() =>
            $"{nameof(_networkName)}: {(string.IsNullOrEmpty(_networkName) ? "N/A" : _networkName)}, {nameof(Placement)}: {Placement}, {nameof(IsClicked)}: {IsClicked}";
    }
}
