// <copyright file="ServerInfoResponse.cs" company="Basho Technologies, Inc.">
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
    /// <summary>
    /// Response to a <see cref="FetchServerInfo"/> command.
    /// </summary>
    public class ServerInfoResponse : Response<ServerInfo>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServerInfoResponse"/> class.
        /// </summary>
        public ServerInfoResponse()
            : base(true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerInfoResponse"/> class.
        /// </summary>
        /// <param name="value">The fetched server information.</param>
        public ServerInfoResponse(ServerInfo value)
            : base(false, value)
        {
        }
    }
}