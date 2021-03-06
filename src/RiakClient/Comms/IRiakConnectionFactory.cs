// <copyright file="IRiakConnectionFactory.cs" company="Basho Technologies, Inc.">
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

namespace RiakClient.Comms
{
    using Config;

    /// <summary>
    /// An interface for factories that create <see cref="IRiakConnection"/>s from <see cref="IRiakNodeConfiguration"/>s.
    /// </summary>
    public interface IRiakConnectionFactory
    {
        /// <summary>
        /// Create a <see cref="IRiakConnection"/> from a <see cref="IRiakNodeConfiguration"/>.
        /// </summary>
        /// <param name="nodeConfig">The node configuration to base the connection on.</param>
        /// <param name="authConfig">The authentication configuration to use with the connection.</param>
        /// <returns>A new instance of a <see cref="IRiakConnection"/> type.</returns>
        IRiakConnection CreateConnection(IRiakNodeConfiguration nodeConfig, IRiakAuthenticationConfiguration authConfig);
    }
}
