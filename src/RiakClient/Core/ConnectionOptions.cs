namespace Riak.Core
{
    using System;
    using System.Net;
    using RiakClient;

    internal class ConnectionOptions
    {
        private readonly IPAddress address;
        private readonly ushort port;
        private readonly Timeout connectTimeout = Constants.DefaultConnectTimeout;
        private readonly Timeout requestTimeout = Constants.DefaultRequestTimeout;

        public ConnectionOptions(IPAddress address, ushort port)
        {
            if (address == null)
            {
                throw new ArgumentNullException("address");
            }

            if (port == 0)
            {
                throw new ArgumentException("port");
            }

            this.address = address;
            this.port = port;
        }

        public ConnectionOptions(string address, ushort port)
            : this(IPAddress.Parse(address), port)
        {
        }

        public ConnectionOptions(IPAddress address, ushort port, Timeout connectTimeout, Timeout requestTimeout)
            : this(address, port)
        {
            this.connectTimeout = connectTimeout;
            this.requestTimeout = requestTimeout;
        }

        public IPAddress Address
        {
            get { return address; }
        }

        public ushort Port
        {
            get { return port; }
        }

        public Timeout ConnectTimeout
        {
            get { return connectTimeout; }
        }

        public Timeout RequestTimeout
        {
            get { return requestTimeout; }
        }
    }
}