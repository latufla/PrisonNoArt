namespace Honeylab.Utils.Ads
{
    public readonly struct RewardedVideoLoadFailedEventArgs : IMessageAccess, IErrorCodeAccess, IAttemptAccess
    {
        public readonly IRewardedVideoService Service;
        private readonly string _message;
        private readonly int? _errorCode;
        private readonly int? _attempt;


        public RewardedVideoLoadFailedEventArgs(IRewardedVideoService service)
            : this(service, null, null, null) { }


        private RewardedVideoLoadFailedEventArgs(IRewardedVideoService service,
            string message,
            int? errorCode,
            int? attempt)
        {
            Service = service;
            _message = message;
            _errorCode = errorCode;
            _attempt = attempt;
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


        public bool TryGetAttempt(out int attempt)
        {
            attempt = _attempt.GetValueOrDefault();
            return _attempt.HasValue;
        }


        public RewardedVideoLoadFailedEventArgs WithMessage(string message) =>
            new RewardedVideoLoadFailedEventArgs(Service, message, _errorCode, _attempt);


        public RewardedVideoLoadFailedEventArgs WithErrorCode(int errorCode) =>
            new RewardedVideoLoadFailedEventArgs(Service, _message, errorCode, _attempt);


        public RewardedVideoLoadFailedEventArgs WithAttempt(int attempt) =>
            new RewardedVideoLoadFailedEventArgs(Service, _message, _errorCode, attempt);


        public override string ToString() =>
            $"{nameof(_message)}: {_message}, {nameof(_errorCode)}: {_errorCode}, {nameof(_attempt)}: {_attempt}";
    }
}
