// <copyright file="FetchCommand{TResponse}.cs" company="Basho Technologies, Inc.">
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

namespace RiakClient.Commands.CRDT
{
    using System;
    using Messages;

    /// <summary>
    /// Fetches a Map from Riak
    /// </summary>
    /// <typeparam name="TResponse">The type of the response data from Riak.</typeparam>
    public abstract class FetchCommand<TResponse> : IRiakCommand where TResponse : Response
    {
        protected readonly FetchCommandOptions Options;

        /// <summary>
        /// Initializes a new instance of the <see cref="FetchCommand{TResponse}"/> class.
        /// </summary>
        /// <param name="options">Options for this operation. See <see cref="FetchCommandOptions"/></param>
        public FetchCommand(FetchCommandOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException("options");
            }

            this.Options = options;
        }

        /// <summary>
        /// The expected protobuf message code from Riak.
        /// </summary>
        public MessageCode ExpectedCode
        {
            get { return MessageCode.DtFetchResp; }
        }

        /// <summary>
        /// A sub-class instance of <see cref="Response"/> representing the response from Riak.
        /// </summary>
        public TResponse Response { get; protected set; }

        public RpbReq ConstructPbRequest()
        {
            var req = new DtFetchReq();

            req.type = Options.BucketType;
            req.bucket = Options.Bucket;
            req.key = Options.Key;

            req.r = Options.R;
            req.pr = Options.PR;

            req.timeout = (uint)Options.Timeout;
            req.notfound_ok = Options.NotFoundOK;
            req.include_context = Options.IncludeContext;
            req.basic_quorum = Options.UseBasicQuorum;

            return req;
        }

        public abstract void OnSuccess(RpbResp response);
    }
}