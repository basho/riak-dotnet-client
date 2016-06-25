namespace RiakClient.Commands
{
    using System;

    public class ServerInfo
    {
        private readonly RiakString node;
        private readonly RiakString serverVersion;

        public ServerInfo(RiakString node, RiakString serverVersion)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }
            else
            {
                this.node = node;
            }

            if (serverVersion == null)
            {
                throw new ArgumentNullException("serverVersion");
            }
            else
            {
                this.serverVersion = serverVersion;
            }
        }

        public RiakString Node
        {
            get { return node; }
        }

        public RiakString ServerVersion
        {
            get { return serverVersion; }
        }
    }
}
