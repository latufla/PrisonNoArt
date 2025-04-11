using Honeylab.Gameplay.World;
using System.Threading;


namespace Honeylab.Utils.PushNotifications
{
    public interface IPushNotificationService
    {
        public void Init(CancellationToken ct);
        public void RegisterUnlockNotification(WorldObjectId unlockId, float delay);
        public void RegisterCraftNotification(WorldObjectId craftBuildingId, float delay = 1.0f);
        public void RegisterUpgradeNotification(float delay);
    }
}
