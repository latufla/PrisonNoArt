using System;
using System.Collections.Generic;


namespace Honeylab.Utils.Extensions
{
    public static class ReadOnlyListExtension
    {
        public static bool ContainsNonAlloc<T>(this IReadOnlyList<T> list, T item)
        {
            var equalityComparer = EqualityComparer<T>.Default;
            for (int i = 0; i < list.Count; i++)
            {
                if (equalityComparer.Equals(list[i], item))
                {
                    return true;
                }
            }

            return false;
        }


        public static T Find<T>(this IReadOnlyList<T> list, Predicate<T> predicate)
        {
            for (int i = 0; i < list.Count; i++)
            {
                T item = list[i];
                if (predicate(item))
                {
                    return item;
                }
            }

            return default;
        }


        public static int SumNonAlloc(this IReadOnlyList<int> list)
        {
            int sum = 0;
            for (int i = 0; i < list.Count; i++)
            {
                sum += list[i];
            }

            return sum;
        }


        public static float SumNonAlloc(this IReadOnlyList<float> list)
        {
            float sum = 0;
            for (int i = 0; i < list.Count; i++)
            {
                sum += list[i];
            }

            return sum;
        }


        public static double SumNonAlloc(this IReadOnlyList<double> list)
        {
            double sum = 0;
            for (int i = 0; i < list.Count; i++)
            {
                sum += list[i];
            }

            return sum;
        }


        public static int IndexOf<T>(this IReadOnlyList<T> list, T item, IEqualityComparer<T> comparer = null)
        {
            const int notFoundValue = -1;
            if (list.Count == 0)
            {
                return notFoundValue;
            }

            var resolvedComparer = comparer ?? EqualityComparer<T>.Default;
            for (int i = 0; i < list.Count; i++)
            {
                if (resolvedComparer.Equals(list[i], item))
                {
                    return i;
                }
            }

            return notFoundValue;
        }


        public static int FindIndex<T>(this IReadOnlyList<T> list, Predicate<T> predicate)
        {
            const int notFoundValue = -1;
            if (list.Count == 0)
            {
                return notFoundValue;
            }

            for (int i = 0; i < list.Count; i++)
            {
                if (predicate(list[i]))
                {
                    return i;
                }
            }

            return notFoundValue;
        }


        public static bool AnyNonAlloc<T>(this IReadOnlyList<T> list, Predicate<T> predicate) =>
            FindIndex(list, predicate) >= 0;
    }
}
