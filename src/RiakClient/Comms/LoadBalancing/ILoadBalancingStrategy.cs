namespace RiakClient.Comms.LoadBalancing
{
    using System.Collections.Generic;

    /// <summary>
    /// The interface for implementing Riak node load balancing strategies against.
    /// </summary>
    public interface ILoadBalancingStrategy
    {
        /// <summary>
        /// Initialise the load balancer with a set of <see cref="IRiakNode"/> nodes.
        /// </summary>
        /// <param name="nodes">The nodes to use with the load balancer.</param>
        void Initialise(IEnumerable<IRiakNode> nodes);

        /// <summary>
        /// Get the next node to use from the load balancer.
        /// </summary>
        /// <returns>A <see cref="IRiakNode"/> to use.</returns>
        IRiakNode SelectNode();

        /// <summary>
        /// Remove a node from the load balancer.
        /// </summary>
        /// <param name="node">The node to remove.</param>
        void RemoveNode(IRiakNode node);

        /// <summary>
        /// Add a node to the load balancer.
        /// </summary>
        /// <param name="node">The node to add.</param>
        void AddNode(IRiakNode node);
    }
}
