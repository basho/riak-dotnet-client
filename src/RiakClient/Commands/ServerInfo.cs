// <copyright file="ServerInfo.cs" company="Basho Technologies, Inc.">
// Copyright 2015 - Basho Technologies, Inc.
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