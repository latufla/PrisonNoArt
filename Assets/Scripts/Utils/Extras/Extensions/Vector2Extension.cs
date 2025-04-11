using UnityEngine;
using Random = System.Random;


namespace Honeylab.Utils.Extensions
{
    public static class Vector2Extension
    {
        public static Vector2 SetX(this Vector2 vec, float x) => new Vector2(x, vec.y);
        public static Vector2 SetY(this Vector2 vec, float y) => new Vector2(vec.x, y);
        public static Vector3 SetZ(this Vector2 vec, float z) => new Vector3(vec.x, vec.y, z);


        public static Vector2 Add(this Vector2 vec, Vector2 addition) => vec + addition;
        public static Vector2 Sub(this Vector2 vec, Vector2 subtraction) => vec - subtraction;


        public static Vector2 ScaleByVector(this Vector2 vec, Vector2 scaler) => Vector2.Scale(vec, scaler);
        public static Vector2 ScaleByScalar(this Vector2 vec, float scalar) => scalar * vec;


        public static Vector2 PerComponentReciprocal(this Vector2 vec) => new Vector2(1.0f / vec.x, 1.0f / vec.y);


        public static float YieldRangeRandom(this Vector2 range)
        {
            float value = UnityEngine.Random.value;
            return Mathf.Lerp(range.x, range.y, value);
        }


        public static float YieldRangeRandom(this Vector2 range, Random random)
        {
            float value = (float)random.NextDouble();
            return Mathf.Lerp(range.x, range.y, value);
        }
    }
}
