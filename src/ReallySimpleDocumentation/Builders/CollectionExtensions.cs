using System.Collections.Generic;

namespace Marsman.ReallySimpleDocumentation.CollectionExtensions
{
    public static class CollectionExtensions
    {
        public static void AddRange<T>(this List<T> list, params T[] items)
        {
            list.AddRange(items);
        }

        public static void AddRange<T1, T2>(this Dictionary<T1, T2> dict, params (T1 key, T2 value)[] items)
        {
            foreach (var item in items)
            {
                dict.Add(item.key, item.value);
            }
        }

        public static void AddRange<T1, T2>(this Dictionary<T1, T2> dict, IEnumerable<KeyValuePair<T1, T2>> items)
        {
            foreach (var item in items)
            {
                dict.Add(item.Key, item.Value);
            }
        }
    }
}
