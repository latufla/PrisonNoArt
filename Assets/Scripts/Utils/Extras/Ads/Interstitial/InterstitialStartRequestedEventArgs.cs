namespace Honeylab.Utils.Ads
{
    public readonly struct InterstitialStartRequestedEventArgs : IPlacementAccess
    {
        public readonly IInterstitialService Service;
        public readonly InterstitialStartResult Result;


        public InterstitialStartRequestedEventArgs(IInterstitialService service,
            string placement,
            InterstitialStartResult result)
        {
            Service = service;
            Placement = placement;
            Result = result;
        }


        public string Placement { get; }


        public override string ToString() => $"{nameof(Result)}: {Result}, {nameof(Placement)}: {Placement}";
    }
}
