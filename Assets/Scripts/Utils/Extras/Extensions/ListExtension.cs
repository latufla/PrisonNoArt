using System;
using System.Collections.Generic;


namespace Honeylab.Utils.Extensions
{
    public static class ListExtension
    {
        public static void Shuffle<T>(this IList<T> list, Random random)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = random.Next(n + 1);
                (list[k], list[n]) = (list[n], list[k]);
            }
        }


        public static int SumNonAlloc(this IList<int> list)
        {
            int sum = 0;
            for (int i = 0; i < list.Count; i++)
            {
                sum += list[i];
            }

            return sum;
        }


        public static float SumNonAlloc(this IList<float> list)
        {
            float sum = 0;
            for (int i = 0; i < list.Count; i++)
            {
                sum += list[i];
            }

            return sum;
        }


        public static double SumNonAlloc(this IList<double> list)
        {
            double sum = 0;
            for (int i = 0; i < list.Count; i++)
            {
                sum += list[i];
            }

            return sum;
        }
    }
}
