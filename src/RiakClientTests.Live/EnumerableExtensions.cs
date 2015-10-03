namespace RiakClientTests.Live
{
    using System;
    using System.Collections.Generic;

    internal static class EnumerableExtensions
    {
        public static IEnumerable<T> Times<T>(this int count, Func<T> generator)
        {
            while (count-- > 0)
            {
                yield return generator();
            }
        }
    }
}
