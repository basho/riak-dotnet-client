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

    public abstract class UpdateCommand<TResponse> : Command<UpdateCommandOptions, TResponse>
        where TResponse : Response
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateCommand{TResponse}"/> class.
        /// </summary>
        /// <param name="options">Options for this operation. See <see cref="UpdateMapOptions"/></param>
        public UpdateCommand(UpdateCommandOptions options)
            : base(options)
        {
            if (this.CommandOptions.HasRemoves &&
                EnumerableUtil.IsNullOrEmpty(this.CommandOptions.Context))
            {
                throw new InvalidOperationException("When doing any removes a context must be provided.");
            }
        }

        public override MessageCode ExpectedCode
        {
            get { return MessageCode.DtUpdateResp; }
        }

        public override RpbReq ConstructPbRequest()
        {
            var req = new DtUpdateReq();

            req.type = CommandOptions.BucketType;
            req.bucket = CommandOptions.Bucket;
            req.key = CommandOptions.Key;

            req.w = CommandOptions.W;
            req.pw = CommandOptions.PW;
            req.dw = CommandOptions.DW;

            req.return_body = CommandOptions.ReturnBody;

            req.timeout = (uint)CommandOptions.Timeout;

            req.context = CommandOptions.Context;
            req.include_context = CommandOptions.IncludeContext;

            if (req.include_context)
            {
                req.return_body = true;
            }

            req.op = GetRequestOp();

            return req;
        }

        public override void OnSuccess(RpbResp response)
        {
            if (response == null)
            {
                Response = default(TResponse);
            }
            else
            {
                DtUpdateResp resp = (DtUpdateResp)response;
                RiakString key = EnumerableUtil.NotNullOrEmpty(resp.key) ?
                    new RiakString(resp.key) : CommandOptions.Key;
                Response = CreateResponse(key, resp);
            }
        }

        protected abstract DtOp GetRequestOp();

        protected abstract TResponse CreateResponse(RiakString key, DtUpdateResp resp);
    }
}