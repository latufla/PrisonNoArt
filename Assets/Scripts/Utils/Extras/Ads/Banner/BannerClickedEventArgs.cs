namespace Honeylab.Utils.Ads
{
    public class BannerClickedEventArgs : INetworkNameAccess
    {
        public readonly IBannerService Service;
        private readonly string _networkName;


        public BannerClickedEventArgs(IBannerService service)
            : this(service, null) { }


        private BannerClickedEventArgs(IBannerService service, string networkName)
        {
            Service = service;
            _networkName = networkName;
        }


        public bool TryGetNetworkName(out string networkName)
        {
            networkName = _networkName;
            return !string.IsNullOrEmpty(networkName);
        }


        public BannerClickedEventArgs WithNetworkName(string networkName) =>
            new BannerClickedEventArgs(Service, networkName);


        public override string ToString() =>
            $"{nameof(_networkName)}: {(string.IsNullOrEmpty(_networkName) ? "N/A" : _networkName)}";
    }
}
