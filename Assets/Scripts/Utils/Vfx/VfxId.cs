using Honeylab.Utils.Data;
using System;
using UnityEngine;


namespace Honeylab.Utils.Vfx
{
    [CreateAssetMenu(fileName = DataUtil.DefaultFileNamePrefix + nameof(VfxId),
        menuName = DataUtil.MenuNamePrefix + "Vfx ID")]
    public class VfxId : ScriptableId, IEquatable<VfxId>
    {
        public bool Equals(VfxId other) => base.Equals(other);


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

            return obj.GetType() == GetType() && Equals((VfxId)obj);
        }


        public override int GetHashCode() => base.GetHashCode();
    }
}
