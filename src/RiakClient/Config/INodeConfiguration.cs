namespace Riak.Config
{
    using System;

    /// <summary>
    /// Represents a configuration for a Riak Node.
    /// </summary>
    public interface INodeConfiguration
    {
        /// <summary>
        /// The name of the node.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// The address of the node.  
        /// Can be an IP address or a resolvable domain name.
        /// </summary>
        string HostAddress { get; set; }

        /// <summary>
        /// The port to use when connecting to the Protocol Buffers API.
        /// </summary>
        int PbcPort { get; set; }

        /// <summary>
        /// The worker pool size to use for this node.
        /// This many workers (and connections) will be available for concurrent use.
        /// </summary>
        int PoolSize { get; set; }

        /// <summary>
        /// If True, this node (pointing to a load balancer) will not be marked offline and requests will always be routed to it.
        /// If False (default) and this node is offline, it will be removed from the pool until health check determines it is healthy again.
        /// </summary>
        bool ExternalLoadBalancer { get; set; }

        /// <summary>
        /// The network timeout to use when attempting to read data from Riak.
        /// </summary>
        TimeSpan NetworkReadTimeout { get; set; }

        /// <summary>
        /// The network timeout to use when attempting to write data to Riak.
        /// </summary>
        TimeSpan NetworkWriteTimeout { get; set; }

        /// <summary>
        /// The network timeout to use when attempting to connect to Riak.
        /// </summary>
        TimeSpan NetworkConnectTimeout { get; set; }
    }
}
