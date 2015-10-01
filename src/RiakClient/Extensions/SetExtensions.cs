namespace RiakClient.Extensions
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    internal static class SetExtensions
    {
        private static readonly UTF8Encoding UTF8 = new UTF8Encoding();

        public static ISet<byte[]> GetUTF8Bytes(this ISet<string> strings)
        {
            return GetBytes(strings, UTF8);
        }

        public static ISet<byte[]> GetBytes(this ISet<string> strings, Encoding encoding)
        {
            ISet<byte[]> rv = null;

            if (strings != null)
            {
                rv = new HashSet<byte[]>(strings.Select(encoding.GetBytes));
            }

            return rv;
        }

        public static ISet<string> GetUTF8Strings(this ISet<byte[]> bytes)
        {
            return GetStrings(bytes, UTF8);
        }

        public static ISet<string> GetStrings(this ISet<byte[]> bytes, Encoding encoding)
        {
            ISet<string> rv = null;

            if (bytes != null)
            {
                rv = new HashSet<string>(bytes.Select(encoding.GetString));
            }

            return rv;
        }
    }
}
