namespace Honeylab.Utils.Ads
{
    internal interface IMessageAccess
    {
        bool TryGetMessage(out string message);
    }
}
