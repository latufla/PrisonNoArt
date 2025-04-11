using UnityEngine;


namespace Honeylab.Utils.Extensions
{
    public static class Vector3Extension
    {
        public static Vector3 SetX(this Vector3 vec, float x)
        {
            vec.x = x;
            return vec;
        }


        public static Vector3 SetY(this Vector3 vec, float y)
        {
            vec.y = y;
            return vec;
        }


        public static Vector3 SetZ(this Vector3 vec, float z)
        {
            vec.z = z;
            return vec;
        }


        public static Vector3 Add(this Vector3 vec, Vector3 addition) => vec + addition;
        public static Vector3 Sub(this Vector3 vec, Vector3 subtraction) => vec - subtraction;


        public static Vector3 ScaleByVector(this Vector3 vec, Vector3 scaler) => Vector3.Scale(vec, scaler);
        public static Vector3 ScaleByScalar(this Vector3 vec, float scalar) => scalar * vec;


        public static Vector3 PerComponentReciprocal(this Vector3 vec) =>
            new Vector3(1.0f / vec.x, 1.0f / vec.y, 1.0f / vec.z);
    }
}
