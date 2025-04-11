using System.Collections.Generic;
using UnityEngine;


namespace Honeylab.Utils.Extensions
{
    // https://forum.unity.com/threads/lerp-from-one-gradient-to-another.342561/#post-3719203
    public static class GradientLerp
    {
        public static Gradient Lerp(Gradient a, Gradient b, float t) => Lerp(a, b, t, false, false);


        public static Gradient LerpNoAlpha(Gradient a, Gradient b, float t) => Lerp(a, b, t, true, false);


        public static Gradient LerpNoColor(Gradient a, Gradient b, float t) => Lerp(a, b, t, false, true);


        private static Gradient Lerp(Gradient a,
            Gradient b,
            float t,
            bool noAlpha,
            bool noColor)
        {
            //list of all the unique key times
            var keysTimes = new List<float>();

            if (!noColor)
            {
                for (int i = 0; i < a.colorKeys.Length; i++)
                {
                    float k = a.colorKeys[i].time;
                    if (!keysTimes.Contains(k))
                        keysTimes.Add(k);
                }

                for (int i = 0; i < b.colorKeys.Length; i++)
                {
                    float k = b.colorKeys[i].time;
                    if (!keysTimes.Contains(k))
                        keysTimes.Add(k);
                }
            }

            if (!noAlpha)
            {
                for (int i = 0; i < a.alphaKeys.Length; i++)
                {
                    float k = a.alphaKeys[i].time;
                    if (!keysTimes.Contains(k))
                        keysTimes.Add(k);
                }

                for (int i = 0; i < b.alphaKeys.Length; i++)
                {
                    float k = b.alphaKeys[i].time;
                    if (!keysTimes.Contains(k))
                        keysTimes.Add(k);
                }
            }

            var colors = new GradientColorKey[keysTimes.Count];
            var alphas = new GradientAlphaKey[keysTimes.Count];

            //Pick colors of both gradients at key times and lerp them
            for (int i = 0; i < keysTimes.Count; i++)
            {
                float time = keysTimes[i];
                Color color = Color.Lerp(a.Evaluate(time), b.Evaluate(time), t);
                colors[i] = new GradientColorKey(color, time);
                alphas[i] = new GradientAlphaKey(color.a, time);
            }

            Gradient g = new Gradient
            {
                mode = a.mode
            };
            g.SetKeys(colors, alphas);

            return g;
        }
    }
}
