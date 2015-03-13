// <copyright file="ILoadBalancingStrategy.cs" company="Basho Technologies, Inc.">
// Copyright (c) 2011 - OJ Reeves & Jeremiah Peschka
// Copyright (c) 2014 - Basho Technologies, Inc.
//
// This file is provided to you under the Apache License,
// Version 2.0 (the "License"); you may not use this file
// except in compliance with the License.  You may obtain
// a copy of the License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing,
// software distributed under the License is distributed on an
// "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
// KIND, either express or implied.  See the License for the
// specific language governing permissions and limitations
// under the License.
// </copyright>

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
