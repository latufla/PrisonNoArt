using Honeylab.Utils.Ads;
using System.Collections.Generic;
using static Honeylab.Utils.Analytics.CommonAnalytics.Messages;


namespace Honeylab.Utils.Analytics
{
    internal class AdsMessages
    {
        public static Dictionary<string, object> CreateMessage(object args)
        {
            var result = new Dictionary<string, object>();

            if (args is IPlacementAccess placementAccess)
            {
                result.Add(Placement, placementAccess.Placement);
            }

            if (args is INetworkNameAccess networkNameAccess &&
                networkNameAccess.TryGetNetworkName(out string networkName))
            {
                result.Add(AdNetwork, networkName);
            }

            if (args is IClickedStateAccess clickedStateAccess &&
                clickedStateAccess.IsClicked.HasValue)
            {
                result.Add(IsClicked, clickedStateAccess.IsClicked.Value);
            }

            if (args is IMessageAccess messageAccess &&
                messageAccess.TryGetMessage(out string message))
            {
                result.Add(Message, message);
            }

            if (args is IErrorCodeAccess errorCodeAccess &&
                errorCodeAccess.TryGetErrorCode(out int errorCode))
            {
                result.Add(ErrorCode, errorCode);
            }

            if (args is IAttemptAccess attemptAccess &&
                attemptAccess.TryGetAttempt(out int attempt))
            {
                result.Add(Attempt, attempt);
            }

            return result;
        }
    }
}
