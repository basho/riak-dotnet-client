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
    using Messages;

    /// <summary>
    /// Fetches a CRDT from Riak
    /// </summary>
    /// <typeparam name="TResponse">The type of the response data from Riak.</typeparam>
    public abstract class FetchCommand<TResponse> : Command<FetchCommandOptions, TResponse>
        where TResponse : Response
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FetchCommand{TResponse}"/> class.
        /// </summary>
        /// <param name="options">Options for this operation. See <see cref="FetchCommandOptions"/></param>
        public FetchCommand(FetchCommandOptions options)
            : base(options)
        {
        }

        /// <summary>
        /// The expected protobuf message code from Riak.
        /// </summary>
        public override MessageCode ExpectedCode
        {
            get { return MessageCode.DtFetchResp; }
        }

        public override RpbReq ConstructPbRequest()
        {
            var req = new DtFetchReq();

            req.type = CommandOptions.BucketType;
            req.bucket = CommandOptions.Bucket;
            req.key = CommandOptions.Key;

            req.r = CommandOptions.R;
            req.pr = CommandOptions.PR;

            req.timeout = (uint)CommandOptions.Timeout;

            req.notfound_ok = CommandOptions.NotFoundOK;
            req.include_context = CommandOptions.IncludeContext;
            req.basic_quorum = CommandOptions.UseBasicQuorum;

            return req;
        }
    }
}