using System;
using System.Collections.Generic;


namespace Honeylab.Utils.Extensions
{
    public static class DictionaryExtension
    {
        public static KeyValuePair<K, V> FirstNonAlloc<K, V>(this Dictionary<K, V> dictionary)
        {
            foreach (var kvp in dictionary)
            {
                return kvp;
            }

            throw new InvalidOperationException();
        }
    }
}
