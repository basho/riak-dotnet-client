namespace RiakClient.Util
{
    using System;

    public static class DateTimeUtil
    {
        private static readonly DateTime Epoch =
            new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static long ToUnixTimeMillis(DateTime date)
        {
            var diff = date.ToUniversalTime() - Epoch;
            return Convert.ToInt64(diff.TotalMilliseconds);
        }

        public static DateTime FromUnixTimeMillis(long unixTime)
        {
            return Epoch.AddMilliseconds(unixTime);
        }
    }
}
