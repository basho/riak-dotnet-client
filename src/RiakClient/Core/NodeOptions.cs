namespace Riak.Core
{
    using System;
    using System.Net;
    using RiakClient.Commands;

    public class NodeOptions
    {
        private readonly IPEndPoint address;
        private readonly ushort minConnections;
        private readonly ushort maxConnections;
        private readonly TimeSpan idleTimeout;
        private readonly TimeSpan connectTimeout;
        private readonly TimeSpan requestTimeout;
        private readonly TimeSpan healthCheckInterval;
        private readonly IRCommandBuilder healthCheckBuilder;

        public NodeOptions(
            IPEndPoint address,
            int minConnections = Constants.DefaultMinConnections,
            int maxConnections = Constants.DefaultMaxConnections,
            TimeSpan idleTimeout = default(TimeSpan),
            TimeSpan connectTimeout = default(TimeSpan),
            TimeSpan requestTimeout = default(TimeSpan),
            TimeSpan healthCheckInterval = default(TimeSpan),
            IRCommandBuilder healthCheckBuilder = null)
        {
            this.address = address;
            if (this.address == null)
            {
                throw new ArgumentNullException(
                    "address",
                    Properties.Resources.Riak_Core_NodeRequiresAddressException);
            }

            if (this.address.Port <= 0 || this.address.Port > ushort.MaxValue)
            {
                throw new ArgumentException(
                    "port",
                    Properties.Resources.Riak_Core_PortMustBeInRange);
            }

            if (minConnections == default(ushort))
            {
                minConnections = Constants.DefaultMinConnections;
            }

            if (maxConnections == default(ushort))
            {
                maxConnections = Constants.DefaultMaxConnections;
            }

            if (minConnections < Constants.DefaultMinConnections)
            {
                throw new ArgumentException(
                    "minConnections",
                    Properties.Resources.Riak_Core_NodeMinConnectionsMustBeOneOrGreaterException);
            }

            if (maxConnections > ushort.MaxValue)
            {
                string message = string.Format(
                    Properties.Resources.Riak_Core_NodeMaxConnectionsMustBeLessThanException_fmt,
                    ushort.MaxValue);
                throw new ArgumentException("minConnections", message);
            }

            if (minConnections > maxConnections)
            {
                throw new ArgumentException(
                    "minConnections",
                    Properties.Resources.Riak_Core_MaxConnectionsMustBeGreaterThanMinConnectionsException);
            }

            this.minConnections = (ushort)minConnections;
            this.maxConnections = (ushort)maxConnections;

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

            this.healthCheckInterval = healthCheckInterval;
            if (this.healthCheckInterval == default(TimeSpan))
            {
                this.healthCheckInterval = Constants.DefaultHealthCheckInterval;
            }

            this.healthCheckBuilder = healthCheckBuilder;
            if (this.healthCheckBuilder == null)
            {
                this.healthCheckBuilder = new Ping.Builder();
            }
        }

        public IPEndPoint Address
        {
            get { return address; }
        }

        public int MinConnections
        {
            get { return minConnections; }
        }

        public int MaxConnections
        {
            get { return maxConnections; }
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

        public TimeSpan HealthCheckInterval
        {
            get { return healthCheckInterval; }
        }

        public IRCommandBuilder HealthCheckBuilder
        {
            get { return healthCheckBuilder; }
        }
    }
}
