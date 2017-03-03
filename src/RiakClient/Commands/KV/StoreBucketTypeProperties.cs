// <copyright file="StoreBucketTypeProperties.cs" company="Basho Technologies, Inc.">
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

    public class StoreBucketTypeProperties : Command<StoreBucketTypePropertiesOptions, StoreBucketTypePropertiesResponse>
    {
        public StoreBucketTypeProperties(RiakString bucketType, RiakBucketProperties bucketProperties)
            : base(new StoreBucketTypePropertiesOptions(bucketType, bucketProperties))
        {
        }

        public override MessageCode ExpectedCode
        {
            get { return MessageCode.RpbSetBucketResp; }
        }

        public override RiakReq ConstructRequest(bool useTtb)
        {
            var req = new RpbSetBucketTypeReq
            {
                type = CommandOptions.BucketType,
                props = CommandOptions.BucketProperties.ToMessage()
            };
            return req;
        }

        public override void OnSuccess(RiakResp response)
        {
            Response = new StoreBucketTypePropertiesResponse(response == null);
        }
    }
}
