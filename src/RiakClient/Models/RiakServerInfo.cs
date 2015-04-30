// <copyright file="RiakServerInfo.cs" company="Basho Technologies, Inc.">
// Copyright 2011 - OJ Reeves & Jeremiah Peschka
// Copyright 2014 - Basho Technologies, Inc.
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

namespace RiakClient.Models
{
    using Extensions;
    using Messages;

    /// <summary>
    /// A collection of server info that can be queried from a single Riak node.
    /// </summary>
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
