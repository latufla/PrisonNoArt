namespace Honeylab.Utils.Ads
{
    internal interface IAttemptAccess
    {
        bool TryGetAttempt(out int attempt);
    }
}
