// <copyright file="IRiakClusterConfiguration.cs" company="Basho Technologies, Inc.">
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

namespace RiakClient.Config
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents a configuration element interface for a Riak Cluster.
    /// </summary>
    public interface IRiakClusterConfiguration
    {
        /// <summary>
        /// A collection of <see cref="IRiakNodeConfiguration"/> configurations detailing the Riak nodes that can be connected to.
        /// </summary>
        IList<IRiakNodeConfiguration> RiakNodes { get; }

        /// <summary>
        /// The period of time to poll nodes for health/liveness checks.
        /// </summary>
        Timeout NodePollTime { get; }

        /// <summary>
        /// The period of time to wait inbetween operation retries.
        /// </summary>
        Timeout DefaultRetryWaitTime { get; }

        /// <summary>
        /// The max number of retry attempts to make when the client encounters 
        /// <see cref="ResultCode"/>.NoConnections or <see cref="ResultCode"/>.CommunicationError errors.
        /// </summary>
        int DefaultRetryCount { get; }

        /// <summary>
        /// A <see cref="IRiakAuthenticationConfiguration"/> configuration that details any authentication information.
        /// </summary>
        IRiakAuthenticationConfiguration Authentication { get; }
    }
}