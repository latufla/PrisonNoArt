using System;
using System.Collections.Generic;
using System.Linq;


namespace Honeylab.Utils
{
    public static class EnumerableExtention
    {
        public static T MaxBy<T, TR>(this IEnumerable<T> en, Func<T, TR> evaluate) where TR : IComparable<TR>
        {
            return en.Select(t => new Tuple<T, TR>(t, evaluate(t)))
                .Aggregate((max, next) => next.Item2.CompareTo(max.Item2) > 0 ? next : max)
                .Item1;
        }


        public static T MinBy<T, TR>(this IEnumerable<T> en, Func<T, TR> evaluate) where TR : IComparable<TR>
        {
            return en.Select(t => new Tuple<T, TR>(t, evaluate(t)))
                .Aggregate((max, next) => next.Item2.CompareTo(max.Item2) < 0 ? next : max)
                .Item1;
        }
    }
}
