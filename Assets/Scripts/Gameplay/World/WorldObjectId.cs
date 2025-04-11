using Honeylab.Utils.Persistence;
using System;
using UnityEngine;


namespace Honeylab.Gameplay.World
{
    [CreateAssetMenu(fileName = DataUtil.DefaultFileNamePrefix + nameof(WorldObjectId),
        menuName = DataUtil.MenuNamePrefix + "World Object ID")]
    public class WorldObjectId : PersistenceId, IEquatable<WorldObjectId>
    {
        public bool Equals(WorldObjectId other) => base.Equals(other);


        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            return obj.GetType() == GetType() && Equals((WorldObjectId)obj);
        }


        public override int GetHashCode() => base.GetHashCode();
    }
}
