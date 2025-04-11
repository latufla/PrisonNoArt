using Honeylab.Gameplay.World;
using Honeylab.Utils.Data;
using UnityEngine;


namespace Honeylab.Utils.PushNotifications
{
    [CreateAssetMenu(fileName = DataUtil.DefaultFileNamePrefix + nameof(UnlockPushNotificationData),
        menuName = DataUtil.MenuNamePrefix + "Unlock Push Notifications Data")]
    public class UnlockPushNotificationData : PushNotificationDataBase
    {
        [SerializeField] private WorldObjectId _unlockId;


        public WorldObjectId UnlockId => _unlockId;
    }
}
