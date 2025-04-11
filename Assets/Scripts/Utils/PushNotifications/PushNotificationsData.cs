using Honeylab.Utils.Persistence;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DataUtil = Honeylab.Utils.Data.DataUtil;


namespace Honeylab.Utils.PushNotifications
{
    public abstract class PushNotificationDataBase : ScriptableObject
    {
        [SerializeField] private string _title;
        [SerializeField] private string _text;
        [SerializeField] private string _smallIcon;
        [SerializeField] private string _largeIcon;


        public string Title => _title;
        public string Text => _text;
        public string SmallIcon => _smallIcon;
        public string LargeIcon => _largeIcon;
    }


    [CreateAssetMenu(fileName = DataUtil.DefaultFileNamePrefix + nameof(PushNotificationsData),
        menuName = DataUtil.MenuNamePrefix + "Push Notifications Data")]
    public class PushNotificationsData : ScriptableObject
    {
        [SerializeField] private PersistenceId _persistenceId;
        [SerializeField] private List<PushNotificationDataBase> _pushNotifications;


        public PersistenceId PersistenceId => _persistenceId;
        public List<T> Get<T>() where T : PushNotificationDataBase => _pushNotifications.OfType<T>().ToList();
    }
}
