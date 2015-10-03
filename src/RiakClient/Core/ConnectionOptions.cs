namespace Riak.Core
{
    using System;
    using System.Net;

    internal class ConnectionOptions
    {
        private readonly IPEndPoint address;
        private readonly TimeSpan connectTimeout = Constants.DefaultConnectTimeout;
        private readonly TimeSpan requestTimeout = Constants.DefaultRequestTimeout;

        public ConnectionOptions(IPEndPoint address)
        {
            if (address == null)
            {
                throw new ArgumentNullException("address");
            }

            if (address.Port <= IPEndPoint.MinPort || address.Port > IPEndPoint.MaxPort)
            {
                throw new ArgumentException(Properties.Resources.Riak_Core_ConnectionPortMustBeInRange);
            }

            this.address = address;
        }

        public ConnectionOptions(string address, ushort port)
            : this(new IPEndPoint(IPAddress.Parse(address), port))
        {
        }

        public ConnectionOptions(IPAddress address, ushort port)
            : this(new IPEndPoint(address, port))
        {
        }

        public ConnectionOptions(IPEndPoint address, TimeSpan connectTimeout, TimeSpan requestTimeout)
            : this(address)
        {
            this.connectTimeout = connectTimeout;
            this.requestTimeout = requestTimeout;
        }

        public IPEndPoint Address
        {
            get { return address; }
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
