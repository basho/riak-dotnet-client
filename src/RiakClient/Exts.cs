namespace RiakClient
{
    using System;

    public static class Exts
    {
        private static readonly DateTime Epoch =
            new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static long ToUnixTimeMillis(this DateTime date)
        {
            var diff = date.ToUniversalTime() - Epoch;
            return Convert.ToInt64(diff.TotalMilliseconds);
        }
    }
}
