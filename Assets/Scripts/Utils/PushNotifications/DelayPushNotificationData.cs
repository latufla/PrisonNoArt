using Honeylab.Utils.Data;
using UnityEngine;


namespace Honeylab.Utils.PushNotifications
{
    [CreateAssetMenu(fileName = DataUtil.DefaultFileNamePrefix + nameof(DelayPushNotificationData),
        menuName = DataUtil.MenuNamePrefix + "Delay Push Notifications Data")]
    public class DelayPushNotificationData : PushNotificationDataBase
    {
        [SerializeField] private float _delay;


        public float Delay => _delay;
    }
}
