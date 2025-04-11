namespace Honeylab.Utils.Ads
{
    internal interface INetworkNameAccess
    {
        bool TryGetNetworkName(out string networkName);
    }
}
