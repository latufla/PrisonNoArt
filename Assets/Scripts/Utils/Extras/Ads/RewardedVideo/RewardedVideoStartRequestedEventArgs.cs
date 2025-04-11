namespace Honeylab.Utils.Ads
{
    public readonly struct RewardedVideoStartRequestedEventArgs : IPlacementAccess
    {
        public IRewardedVideoService Service { get; }
        public RewardedVideoStartResult Result { get; }


        public RewardedVideoStartRequestedEventArgs(IRewardedVideoService service,
            string placement,
            RewardedVideoStartResult result)
        {
            Service = service;
            Placement = placement;
            Result = result;
        }


        public string Placement { get; }


        public override string ToString() => $"{nameof(Result)}: {Result}, {nameof(Placement)}: {Placement}";
    }
}
