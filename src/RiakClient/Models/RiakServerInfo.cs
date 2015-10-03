namespace RiakClient.Models
{
    using Extensions;
    using Messages;

    /// <summary>
    /// A collection of server info that can be queried from a single Riak node.
    /// </summary>
    [System.Obsolete("Please use RiakClient.Commands.ServerInfo instead.")]
    public class RiakServerInfo
    {
        private readonly string node;
        private readonly string version;

        internal RiakServerInfo(RpbGetServerInfoResp resp)
        {
            this.node = resp.node.FromRiakString();
            this.version = resp.server_version.FromRiakString();
        }

        /// <summary>
        /// The Riak node's "name".
        /// </summary>
        public string Node
        {
            get { return node; }
        }

        /// <summary>
        /// The Riak node's version string.
        /// </summary>
        public string Version
        {
            get { return version; }
        }
    }
}
