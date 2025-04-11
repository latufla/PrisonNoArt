using UnityEngine;
using Zenject;


namespace Honeylab.Utils.PushNotifications
{
    public class PushNotificationsInstaller : MonoInstaller
    {
        [SerializeField] private PushNotificationsData _data;


        public override void InstallBindings()
        {
            #if UNITY_IOS
            Container.BindInterfacesAndSelfTo<IosPushNotificationService>()
                .AsSingle()
                .WithArguments(_data);
            #else
            Container.BindInterfacesAndSelfTo<AndroidPushNotificationsService>()
                .AsSingle()
                .WithArguments(_data);
            #endif
        }
    }
}
