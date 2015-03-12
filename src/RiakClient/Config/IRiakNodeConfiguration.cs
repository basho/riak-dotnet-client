// <copyright file="IRiakNodeConfiguration.cs" company="Basho Technologies, Inc.">
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
    using System;

    /// <summary>
    /// Represents a configuration element interface for a Riak Node.
    /// </summary>
    public interface IRiakNodeConfiguration
    {
        /// <summary>
        /// The name of the node.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The address of the node.  
        /// Can be an IP address or a resolvable domain name.
        /// </summary>
        string HostAddress { get; }

        /// <summary>
        /// The port to use when connecting to the Protocol Buffers API.
        /// </summary>
        int PbcPort { get; }

        /// <summary>
        /// The worker pool size to use for this node.
        /// This many workers (and connections) will be available for concurrent use.
        /// </summary>
        int PoolSize { get; }

        /// <summary>
        /// The network timeout to use when attempting to read data from Riak.
        /// </summary>
        Timeout NetworkReadTimeout { get; }

        /// <summary>
        /// The network timeout to use when attempting to write data to Riak.
        /// </summary>
        Timeout NetworkWriteTimeout { get; }

        /// <summary>
        /// The network timeout to use when attempting to connect to Riak.
        /// </summary>
        Timeout NetworkConnectTimeout { get; }
    }
}