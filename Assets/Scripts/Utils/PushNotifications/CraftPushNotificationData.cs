using Honeylab.Gameplay.World;
using Honeylab.Utils.Data;
using UnityEngine;


namespace Honeylab.Utils.PushNotifications
{
    [CreateAssetMenu(fileName = DataUtil.DefaultFileNamePrefix + nameof(CraftPushNotificationData),
        menuName = DataUtil.MenuNamePrefix + "Craft Push Notifications Data")]
    public class CraftPushNotificationData : PushNotificationDataBase
    {
        [SerializeField] private WorldObjectId _craftBuildingId;

        public WorldObjectId CraftBuildingId => _craftBuildingId;
    }
}
