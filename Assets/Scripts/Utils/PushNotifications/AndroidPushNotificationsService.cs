#if !UNITY_IOS

using Honeylab.Gameplay.World;
using Honeylab.Persistence;
using Honeylab.Utils.Persistence;
using System;
using System.Linq;
using System.Threading;
using Unity.Notifications.Android;


namespace Honeylab.Utils.PushNotifications
{
    public class AndroidPushNotificationsService : IPushNotificationService
    {
        private readonly PushNotificationsData _data;
        private readonly SharedPersistenceService _sharedPersistenceService;

        private AndroidNotificationChannel _channel;


        public AndroidPushNotificationsService(PushNotificationsData data, SharedPersistenceService sharedPersistenceService)
        {
            _data = data;
            _sharedPersistenceService = sharedPersistenceService;
        }


        public void Init(CancellationToken ct)
        {
            _channel = new AndroidNotificationChannel
            {
                Id = "channels_honeylab_post_id",
                Name = "Channels Honeylab Post",
                Importance = Importance.High,
                Description = "Honeylab Post Notifications"
            };
            AndroidNotificationCenter.RegisterNotificationChannel(_channel);

            RegisterDelayNotifications();
        }


        public void RegisterUnlockNotification(WorldObjectId unlockId, float delay)
        {
            UnlockPushNotificationData unlock = _data.Get<UnlockPushNotificationData>()
                .FirstOrDefault(it => it.UnlockId.Equals(unlockId));
            if (unlock == null)
            {
                return;
            }

            AndroidNotification notification = new AndroidNotification
            {
                Title = unlock.Title,
                Text = unlock.Text,
                SmallIcon = unlock.SmallIcon,
                LargeIcon = unlock.LargeIcon,
                FireTime = DateTime.Now.AddSeconds(delay),
                ShowInForeground = false
            };

            AndroidNotificationCenter.SendNotification(notification, _channel.Id);
        }


        private void RegisterDelayNotifications()
        {
            PushNotificationsPersistentComponent persistence =
                _sharedPersistenceService.GetOrAddComponent<PushNotificationsPersistentComponent>(_data.PersistenceId);

            var notifications = _data.Get<DelayPushNotificationData>();
            foreach (DelayPushNotificationData data in notifications)
            {
                int nameHash = data.name.GetHashCode();
                if (persistence.NameHashToId.TryGetValue(nameHash, out int _))
                {
                    continue;
                }

                AndroidNotification notification = new()
                {
                    Title = data.Title,
                    Text = data.Text,
                    SmallIcon = data.SmallIcon,
                    LargeIcon = data.LargeIcon,
                    FireTime = DateTime.Now.AddSeconds(data.Delay),
                    ShowInForeground = false
                };

                int id = AndroidNotificationCenter.SendNotification(notification, _channel.Id);
                persistence.NameHashToId.Add(nameHash, id);
            }
        }


        public void RegisterCraftNotification(WorldObjectId craftBuildingId, float delay = 1.0f)
        {
            CraftPushNotificationData data = _data.Get<CraftPushNotificationData>()
                .FirstOrDefault(it => it.CraftBuildingId.Equals(craftBuildingId));
            if (data == null)
            {
                return;
            }

            AndroidNotification notification = new AndroidNotification
            {
                Title = data.Title,
                Text = data.Text,
                SmallIcon = data.SmallIcon,
                LargeIcon = data.LargeIcon,
                FireTime = DateTime.Now.AddSeconds(delay),
                ShowInForeground = false
            };

            AndroidNotificationCenter.SendNotification(notification, _channel.Id);
        }


        public void RegisterUpgradeNotification(float delay)
        {
            UpgradePushNotificationData data = _data.Get<UpgradePushNotificationData>().FirstOrDefault();
            if (data == null)
            {
                return;
            }

            AndroidNotification notification = new AndroidNotification
            {
                Title = data.Title,
                Text = data.Text,
                SmallIcon = data.SmallIcon,
                LargeIcon = data.LargeIcon,
                FireTime = DateTime.Now.AddSeconds(delay),
                ShowInForeground = false
            };

            AndroidNotificationCenter.SendNotification(notification, _channel.Id);
        }
    }
}

#endif
