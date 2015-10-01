namespace RiakClient.Util
{
    using System.Collections;
    using System.Collections.Generic;

    internal static class EnumerableUtil
    {
        public static bool NotNullOrEmpty(IEnumerable items)
        {
            return !EnumerableUtil.IsNullOrEmpty(items);
        }

        public static bool NotNullOrEmpty<T>(IEnumerable<T> items)
        {
            return !EnumerableUtil.IsNullOrEmpty(items);
        }

        public static bool IsNullOrEmpty(IEnumerable items)
        {
            if (items == null)
            {
                return true;
            }

            var collection = items as ICollection;
            if (collection != null)
            {
                return collection.Count == 0;
            }

            return !items.GetEnumerator().MoveNext();
        }

        public static bool IsNullOrEmpty<T>(IEnumerable<T> items)
        {
            if (items == null)
            {
                return true;
            }

            var collection = items as ICollection<T>;
            if (collection != null)
            {
                return collection.Count == 0;
            }

            return IsNullOrEmpty((IEnumerable)items);
        }
    }
}
