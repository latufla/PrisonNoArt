namespace Honeylab.Utils.Ads
{
    public readonly struct InterstitialLoadFailedEventArgs : IMessageAccess, IErrorCodeAccess, IAttemptAccess
    {
        public readonly IInterstitialService Service;
        private readonly string _message;
        private readonly int? _errorCode;
        private readonly int? _attempt;


        public InterstitialLoadFailedEventArgs(IInterstitialService service)
            : this(service, null, null, null) { }


        private InterstitialLoadFailedEventArgs(IInterstitialService service,
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


        public InterstitialLoadFailedEventArgs WithMessage(string message) =>
            new InterstitialLoadFailedEventArgs(Service, message, _errorCode, _attempt);


        public InterstitialLoadFailedEventArgs WithErrorCode(int errorCode) =>
            new InterstitialLoadFailedEventArgs(Service, _message, errorCode, _attempt);


        public InterstitialLoadFailedEventArgs WithAttempt(int attempt) =>
            new InterstitialLoadFailedEventArgs(Service, _message, _errorCode, attempt);


        public override string ToString() => $"{nameof(_message)}: {_message}, {nameof(_errorCode)}: {_errorCode}, {nameof(_attempt)}: {_attempt}";
    }
}
