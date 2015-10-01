namespace Riak
{
    using System;

    internal static class Constants
    {
        public const ushort DefaultMinConnections = 1;
        public const ushort DefaultMaxConnections = 8096;

        public static readonly TimeSpan DefaultIdleExpirationInterval = FiveSeconds;
        public static readonly TimeSpan DefaultIdleTimeout = TenSeconds;
        public static readonly TimeSpan DefaultConnectTimeout = ThreeSeconds;
        public static readonly TimeSpan DefaultRequestTimeout = FiveSeconds;
        public static readonly TimeSpan DefaultCommandTimeout = DefaultRequestTimeout;
        public static readonly TimeSpan DefaultHealthCheckInterval = TimeSpan.FromMilliseconds(125);

        public static readonly byte DefaultExecutionAttempts = 3;
        public static readonly TimeSpan DefaultQueueExecutionInterval = TimeSpan.FromMilliseconds(125);

        private static readonly TimeSpan ThreeSeconds = TimeSpan.FromSeconds(3);
        private static readonly TimeSpan FiveSeconds = TimeSpan.FromSeconds(5);
        private static readonly TimeSpan TenSeconds = TimeSpan.FromSeconds(10);
    }
}
