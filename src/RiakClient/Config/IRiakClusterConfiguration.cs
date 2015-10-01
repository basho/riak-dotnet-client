namespace RiakClient.Config
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a configuration element interface for a Riak Cluster.
    /// </summary>
    public interface IRiakClusterConfiguration
    {
        /// <summary>
        /// A collection of <see cref="IRiakNodeConfiguration"/> configurations detailing the Riak nodes that can be connected to.
        /// </summary>
        IEnumerable<IRiakNodeConfiguration> RiakNodes { get; }

        /// <summary>
        /// The period of time to poll nodes for health/liveness checks.
        /// </summary>
        TimeSpan NodePollTime { get; set; }

        /// <summary>
        /// The period of time to wait inbetween operation retries.
        /// </summary>
        TimeSpan DefaultRetryWaitTime { get; set; }

        /// <summary>
        /// The max number of retry attempts to make when the client encounters 
        /// <see cref="ResultCode"/>.NoConnections or <see cref="ResultCode"/>.CommunicationError errors.
        /// </summary>
        int DefaultRetryCount { get; set; }

        /// <summary>
        /// A <see cref="IRiakAuthenticationConfiguration"/> configuration that details any authentication information.
        /// </summary>
        IRiakAuthenticationConfiguration Authentication { get; set; }

        /// <summary>
        /// Add a <see cref="IRiakNodeConfiguration"/> configuration to the cluster configuration.
        /// </summary>
        /// <param name="nodeConfiguration">The node configuration</param>
        void AddNode(IRiakNodeConfiguration nodeConfiguration);
    }
}
