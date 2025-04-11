using UnityEngine;


namespace Honeylab.Utils.Extensions
{
    public static class TransformExtension
    {
        public static Transform LocalResetPosition(this Transform t)
        {
            t.localPosition = Vector3.zero;
            return t;
        }


        public static Transform LocalResetRotation(this Transform t)
        {
            t.localRotation = Quaternion.identity;
            return t;
        }


        public static Transform LocalResetScale(this Transform t)
        {
            t.localScale = Vector3.one;
            return t;
        }


        public static Transform LocalResetAll(this Transform t) => t
            .LocalResetPosition()
            .LocalResetRotation()
            .LocalResetScale();


        public static Transform SetParentChained(this Transform t, Transform parent, bool worldPositionStays = true)
        {
            t.SetParent(parent, worldPositionStays);
            return t;
        }
    }
}
