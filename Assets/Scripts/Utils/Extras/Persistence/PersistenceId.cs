using Honeylab.Utils.Data;
using System;
using UnityEngine;


namespace Honeylab.Utils.Persistence
{
    [CreateAssetMenu(fileName = DataUtil.DefaultFileNamePrefix + nameof(PersistenceId),
        menuName = DataUtil.MenuNamePrefix + "Persistence ID")]
    public class PersistenceId : ScriptableId
    {
        public static PersistenceId CreateWithGuidString(string guidStr)
        {
            PersistenceId id = CreateInstance<PersistenceId>();
            Guid.TryParse(guidStr, out Guid guid);
            id.SetGuid(guid);
            return id;
        }


        public static PersistenceId CreateWithGuid(Guid guid)
        {
            PersistenceId id = CreateInstance<PersistenceId>();
            id.SetGuid(guid);
            return id;
        }
    }
}
