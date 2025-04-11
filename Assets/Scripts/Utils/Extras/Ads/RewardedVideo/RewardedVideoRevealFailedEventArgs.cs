namespace Honeylab.Utils.Ads
{
    public readonly struct RewardedVideoRevealFailedEventArgs : IPlacementAccess, INetworkNameAccess,
        IMessageAccess, IErrorCodeAccess
    {
        public readonly IRewardedVideoService Service;
        private readonly string _networkName;
        private readonly string _message;
        private readonly int? _errorCode;


        public RewardedVideoRevealFailedEventArgs(IRewardedVideoService service, string placement)
            : this(service, placement, null, null, null) { }


        private RewardedVideoRevealFailedEventArgs(IRewardedVideoService service,
            string placement,
            string networkName,
            string message,
            int? errorCode)
        {
            Service = service;
            Placement = placement;
            _networkName = networkName;
            _message = message;
            _errorCode = errorCode;
        }


        public string Placement { get; }


        public bool TryGetNetworkName(out string networkName)
        {
            networkName = _networkName;
            return !string.IsNullOrEmpty(networkName);
        }


        public bool TryGetMessage(out string message)
        {
            message = _message;
            return !string.IsNullOrEmpty(message);
        }


        public bool TryGetErrorCode(out int errorCode)
        {
            errorCode = _errorCode.GetValueOrDefault();
            return _errorCode.HasValue;
        }


        public RewardedVideoRevealFailedEventArgs WithNetworkName(string networkName) =>
            new RewardedVideoRevealFailedEventArgs(Service, Placement, networkName, _message, _errorCode);


        public RewardedVideoRevealFailedEventArgs WithMessage(string message) =>
            new RewardedVideoRevealFailedEventArgs(Service, Placement, _networkName, message, _errorCode);


        public RewardedVideoRevealFailedEventArgs WithErrorCode(int errorCode) =>
            new RewardedVideoRevealFailedEventArgs(Service, _message, _networkName, _message, errorCode);


        public override string ToString() =>
            $"{nameof(_networkName)}: {_networkName}, {nameof(_message)}: {_message}, {nameof(_errorCode)}: {_errorCode}, {nameof(Placement)}: {Placement}";
    }
}
