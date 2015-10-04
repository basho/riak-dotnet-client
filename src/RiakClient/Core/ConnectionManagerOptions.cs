namespace Riak.Core
{
    using System;
    using System.Net;

    internal class ConnectionManagerOptions
    {
        private readonly IPEndPoint address;
        private readonly ushort minConnections;
        private readonly ushort maxConnections;
        private readonly TimeSpan idleExpirationInterval;
        private readonly TimeSpan idleTimeout;
        private readonly TimeSpan connectTimeout;
        private readonly TimeSpan requestTimeout;

        public ConnectionManagerOptions(
            IPEndPoint address,
            ushort minConnections = default(ushort),
            ushort maxConnections = default(ushort),
            TimeSpan idleExpirationInterval = default(TimeSpan),
            TimeSpan idleTimeout = default(TimeSpan),
            TimeSpan connectTimeout = default(TimeSpan),
            TimeSpan requestTimeout = default(TimeSpan)) // TODO 3.0 authentication
        {
            if (address == null)
            {
                throw new ArgumentNullException(
                    "address",
                    Properties.Resources.Riak_Core_ConnectionManagerRequiresAddressException);
            }

            if (minConnections > maxConnections)
            {
                throw new ArgumentException(
                    "minConnections",
                    Properties.Resources.Riak_Core_MaxConnectionsMustBeGreaterThanMinConnectionsException);
            }

            this.address = address;

            this.minConnections = minConnections;
            if (this.minConnections == default(ushort))
            {
                this.minConnections = Constants.DefaultMinConnections;
            }

            this.maxConnections = maxConnections;
            if (this.maxConnections == default(ushort))
            {
                this.maxConnections = Constants.DefaultMaxConnections;
            }

            this.idleExpirationInterval = idleExpirationInterval;
            if (this.idleExpirationInterval == default(TimeSpan))
            {
                this.idleExpirationInterval = Constants.DefaultIdleExpirationInterval;
            }

            this.idleTimeout = idleTimeout;
            if (this.idleTimeout == default(TimeSpan))
            {
                this.idleTimeout = Constants.DefaultIdleTimeout;
            }

            this.connectTimeout = connectTimeout;
            if (this.connectTimeout == default(TimeSpan))
            {
                this.connectTimeout = Constants.DefaultConnectTimeout;
            }

            this.requestTimeout = requestTimeout;
            if (this.requestTimeout == default(TimeSpan))
            {
                this.requestTimeout = Constants.DefaultRequestTimeout;
            }
        }

        public IPEndPoint Address
        {
            get { return address; }
        }

        public ushort MinConnections
        {
            get { return minConnections; }
        }

        public ushort MaxConnections
        {
            get { return maxConnections; }
        }

        public TimeSpan IdleExpirationInterval
        {
            get { return idleExpirationInterval; }
        }

        public TimeSpan IdleTimeout
        {
            get { return idleTimeout; }
        }

        public TimeSpan ConnectTimeout
        {
            get { return connectTimeout; }
        }

        public TimeSpan RequestTimeout
        {
            get { return requestTimeout; }
        }
    }
}
