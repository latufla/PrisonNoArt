using UnityEngine;
using Random = System.Random;


namespace Honeylab.Utils.Extensions
{
    public static class SystemRandomExtension
    {
        public static float NextFloat(this Random random) => (float)random.NextDouble();


        public static float NextFloat(this Random random, float min, float max) =>
            Mathf.Lerp(min, max, NextFloat(random));


        public static double NextDouble(this Random random, double min, double max) =>
            min + (max - min) * random.NextDouble();


        public static bool NextBoolean(this Random random) => random.NextDouble() < 0.5f;
    }
}
