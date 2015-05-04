// <copyright file="UpdateCommand{TResponse}.cs" company="Basho Technologies, Inc.">
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
    using Util;

    public abstract class UpdateCommand<TResponse> : IRiakCommand where TResponse : Response, new()
    {
        private readonly UpdateCommandOptions options;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateCommand{TResponse}"/> class.
        /// </summary>
        /// <param name="options">Options for this operation. See <see cref="UpdateMapOptions"/></param>
        public UpdateCommand(UpdateCommandOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException("options");
            }

            this.options = options;

            if (this.options.HasRemoves &&
                EnumerableUtil.IsNullOrEmpty(this.options.Context))
            {
                throw new InvalidOperationException("When doing any removes a context must be provided.");
            }
        }

        public MessageCode ExpectedCode
        {
            get { return MessageCode.DtUpdateResp; }
        }

        public TResponse Response { get; private set; }

        public RpbReq ConstructPbRequest()
        {
            var req = new DtUpdateReq();

            req.type = options.BucketType;
            req.bucket = options.Bucket;
            req.key = options.Key;

            req.w = options.W;
            req.pw = options.PW;
            req.dw = options.DW;

            req.return_body = options.ReturnBody;

            req.timeout = (uint)options.Timeout.TotalMilliseconds;

            req.context = options.Context;
            req.include_context = options.IncludeContext;

            req.op = GetRequestOp();

            return req;
        }

        public void OnSuccess(RpbResp response)
        {
            if (response == null)
            {
                Response = new TResponse();
            }
            else
            {
                DtUpdateResp resp = (DtUpdateResp)response;
                RiakString key = EnumerableUtil.NotNullOrEmpty(resp.key) ?
                    new RiakString(resp.key) : options.Key;
                Response = CreateResponse(key, resp);
            }
        }

        protected abstract DtOp GetRequestOp();

        protected abstract TResponse CreateResponse(RiakString key, DtUpdateResp resp);
    }
}