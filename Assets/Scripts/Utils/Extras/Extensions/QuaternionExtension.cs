using UnityEngine;


namespace Honeylab.Utils.Extensions
{
    public static class QuaternionExtension
    {
        public static Quaternion SetEulerX(this Quaternion source, float eulerX)
        {
            Vector3 sourceEuler = source.eulerAngles;
            return Quaternion.Euler(eulerX, sourceEuler.y, sourceEuler.z);
        }


        public static Quaternion SetEulerY(this Quaternion source, float eulerY)
        {
            Vector3 sourceEuler = source.eulerAngles;
            return Quaternion.Euler(sourceEuler.x, eulerY, sourceEuler.z);
        }


        public static Quaternion SetEulerZ(this Quaternion source, float eulerZ)
        {
            Vector3 sourceEuler = source.eulerAngles;
            return Quaternion.Euler(sourceEuler.x, sourceEuler.y, eulerZ);
        }
    }
}
