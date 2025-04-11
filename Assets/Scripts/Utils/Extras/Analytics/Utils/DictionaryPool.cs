using System.Collections.Generic;


namespace Honeylab.Utils.Analytics
{
    public static class DictionaryPool<TKey, TValue>
    {
        private static readonly List<Dictionary<TKey, TValue>> Pool = new List<Dictionary<TKey, TValue>>();


        public static Dictionary<TKey, TValue> Pop()
        {
            int poolSize = Pool.Count;
            if (poolSize == 0)
            {
                return new Dictionary<TKey, TValue>();
            }

            int dictionaryIndex = poolSize - 1;
            var dictionary = Pool[dictionaryIndex];
            Pool.RemoveAt(dictionaryIndex);
            return dictionary;
        }


        public static void Push(Dictionary<TKey, TValue> dictionary)
        {
            dictionary.Clear();
            Pool.Add(dictionary);
        }
    }
}
