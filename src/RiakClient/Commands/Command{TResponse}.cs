// <copyright file="Command{TResponse}.cs" company="Basho Technologies, Inc.">
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
    using Messages;

    /// <summary>
    /// Base class for Riak commands that don't have options.
    /// </summary>
    /// <typeparam name="TResponse">The type of the response data from Riak.</typeparam>
    public abstract class Command<TResponse> : IRiakCommand
        where TResponse : Response
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Command{TResponse}"/> class.
        /// </summary>
        public Command()
        {
        }

        /// <summary>
        /// A sub-class instance of <see cref="Response"/> representing the response from Riak.
        /// </summary>
        public TResponse Response { get; protected set; }

        public abstract MessageCode ExpectedCode { get; }

        public abstract RpbReq ConstructPbRequest();

        public abstract void OnSuccess(RpbResp rpbResp);
    }
}