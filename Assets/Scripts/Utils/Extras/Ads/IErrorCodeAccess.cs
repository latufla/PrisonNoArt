namespace Honeylab.Utils.Ads
{
    public interface IErrorCodeAccess
    {
        bool TryGetErrorCode(out int errorCode);
    }
}
