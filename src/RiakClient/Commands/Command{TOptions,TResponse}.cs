// <copyright file="Command{TOptions,TResponse}.cs" company="Basho Technologies, Inc.">
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
    using Messages;

    /// <summary>
    /// Base class for Riak commands.
    /// </summary>
    /// <typeparam name="TOptions">The type of the options for this command.</typeparam>
    /// <typeparam name="TResponse">The type of the response data from Riak.</typeparam>
    public abstract class Command<TOptions, TResponse> : IRiakCommand
        where TOptions : CommandOptions
        where TResponse : Response
    {
        protected readonly TOptions CommandOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="Command{TOptions, TResponse}"/> class.
        /// </summary>
        /// <param name="options">Options for this operation. See <see cref="CommandOptions"/></param>
        public Command(TOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException("options");
            }

            this.CommandOptions = options;
        }

        public TOptions Options
        {
            get { return CommandOptions; }
        }

        /// <summary>
        /// A sub-class instance of <see cref="Response"/> representing the response from Riak.
        /// </summary>
        public TResponse Response { get; protected set; }

        public abstract MessageCode ExpectedCode { get; }

        public abstract RpbReq ConstructPbRequest();

        public abstract void OnSuccess(RpbResp rpbResp);

        protected RiakString GetKey(RpbResp response)
        {
            RiakString key = null;

            IRpbGeneratedKey krsp = response as IRpbGeneratedKey;
            if (krsp != null && krsp.HasKey)
            {
                key = new RiakString(krsp.key);
            }
            else
            {
                key = CommandOptions.Key;
            }

            return key;
        }
    }
}