#if UNITY_IOS

using Cysharp.Threading.Tasks;
using Honeylab.Gameplay.World;
using System;
using System.Linq;
using System.Threading;
using Unity.Notifications.iOS;
using UnityEngine;


namespace Honeylab.Utils.PushNotifications
{
    public class IosPushNotificationService : IPushNotificationService
    {
        private readonly PushNotificationsData _data;

        public IosPushNotificationService(PushNotificationsData data)
        {
            _data = data;
        }


        public void Init(CancellationToken ct)
        {
            RegisterDelayNotifications();

            RequestAuthorization(ct).Forget();
        }


        private async UniTask RequestAuthorization(CancellationToken ct)
        {
            using (var request = new AuthorizationRequest(AuthorizationOption.Alert | AuthorizationOption.Badge, true))
            {
                while (!request.IsFinished)
                {
                    await UniTask.Yield(ct);
                }

                string res = "\n RequestAuthorization: \n";
                res += "\n finished: " + request.IsFinished;
                res += "\n granted :  " + request.Granted;
                res += "\n error:  " + request.Error;
                res += "\n deviceToken:  " + request.DeviceToken;
                Debug.Log(res);
            }
        }


        private void RegisterDelayNotifications()
        {
            var notifications = _data.Get<DelayPushNotificationData>();
            int id = 0;
            foreach (DelayPushNotificationData data in notifications)
            {
                var identifier = "delayPushIdentifier_" + id;

                iOSNotificationCenter.RemoveScheduledNotification(identifier);

                var timeTrigger = new iOSNotificationTimeIntervalTrigger()
                {
                    TimeInterval = new TimeSpan(0, 0, (int)data.Delay),
                    Repeats = false
                };

                var notification = new iOSNotification()
                {
                    Identifier = identifier,
                    Title = data.Title,
                    Body = data.Text,
                    ShowInForeground = false,
                    ForegroundPresentationOption = (PresentationOption.Alert | PresentationOption.Sound),
                    Trigger = timeTrigger,
                };

                iOSNotificationCenter.ScheduleNotification(notification);
                id++;
            }
        }

        public void RegisterUnlockNotification(WorldObjectId unlockId, float delay)
        {
            UnlockPushNotificationData unlock = _data.Get<UnlockPushNotificationData>()
                .FirstOrDefault(it => it.UnlockId.Equals(unlockId));

            if (unlock == null)
            {
                return;
            }

            var timeTrigger = new iOSNotificationTimeIntervalTrigger()
            {
                TimeInterval = new TimeSpan(0, 0, (int)delay),
                Repeats = false
            };

            var notification = new iOSNotification()
            {
                Title = unlock.Title,
                Body = unlock.Text,
                ShowInForeground = false,
                ForegroundPresentationOption = (PresentationOption.Alert | PresentationOption.Sound),
                Trigger = timeTrigger,
            };

            iOSNotificationCenter.ScheduleNotification(notification);
        }


        public void RegisterCraftNotification(WorldObjectId craftBuildingId, float delay = 1.0f)
        {
            CraftPushNotificationData data = _data.Get<CraftPushNotificationData>()
                .FirstOrDefault(it => it.CraftBuildingId.Equals(craftBuildingId));
            if (data == null)
            {
                return;
            }

            var timeTrigger = new iOSNotificationTimeIntervalTrigger()
            {
                TimeInterval = new TimeSpan(0, 0, (int)delay),
                Repeats = false
            };

            var notification = new iOSNotification()
            {
                Title = data.Title,
                Body = data.Text,
                ShowInForeground = false,
                ForegroundPresentationOption = (PresentationOption.Alert | PresentationOption.Sound),
                Trigger = timeTrigger,
            };

            iOSNotificationCenter.ScheduleNotification(notification);
        }


        public void RegisterUpgradeNotification(float delay)
        {
            UpgradePushNotificationData data = _data.Get<UpgradePushNotificationData>().FirstOrDefault();
            if (data == null)
            {
                return;
            }

            var timeTrigger = new iOSNotificationTimeIntervalTrigger()
            {
                TimeInterval = new TimeSpan(0, 0, (int)delay),
                Repeats = false
            };

            var notification = new iOSNotification()
            {
                Title = data.Title,
                Body = data.Text,
                ShowInForeground = false,
                ForegroundPresentationOption = (PresentationOption.Alert | PresentationOption.Sound),
                Trigger = timeTrigger,
            };

            iOSNotificationCenter.ScheduleNotification(notification);
        }
    }
}

#endif
