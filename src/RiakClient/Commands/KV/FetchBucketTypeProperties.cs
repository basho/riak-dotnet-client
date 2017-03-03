// <copyright file="FetchBucketTypeProperties.cs" company="Basho Technologies, Inc.">
// Copyright 2017 - Basho Technologies, Inc.
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

namespace RiakClient.Commands.KV
{
    using Messages;
    using Models;

    public class FetchBucketTypeProperties : Command<FetchBucketTypePropertiesOptions, FetchBucketTypePropertiesResponse>
    {
        public FetchBucketTypeProperties(RiakString bucketType)
            : base(new FetchBucketTypePropertiesOptions(bucketType))
        {
        }

        public override MessageCode ExpectedCode
        {
            get { return MessageCode.RpbGetBucketResp; }
        }

        public override RiakReq ConstructRequest(bool useTtb)
        { 
            var req = new RpbGetBucketTypeReq();
            req.type = CommandOptions.BucketType;
            return req;
        }

        public override void OnSuccess(RiakResp response)
        {
            if (response == null)
            {
                Response = new FetchBucketTypePropertiesResponse();
            }
            else
            {
                RpbGetBucketResp resp = (RpbGetBucketResp)response;

                var props = new RiakBucketProperties(resp.props);

                Response = new FetchBucketTypePropertiesResponse(props);
            }
        }
    }
}
