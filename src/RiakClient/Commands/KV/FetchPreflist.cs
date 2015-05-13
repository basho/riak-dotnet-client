// <copyright file="FetchPreflist.cs" company="Basho Technologies, Inc.">
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

namespace RiakClient.Commands.KV
{
    using System.Collections.Generic;
    using System.Linq;
    using Exceptions;
    using Messages;
    using Util;

    /// <summary>
    /// Fetches a Map from Riak
    /// </summary>
    public class FetchPreflist : Command<FetchPreflistOptions, Response<IEnumerable<PreflistItem>>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FetchPreflist"/> class.
        /// </summary>
        /// <param name="options">Options for this operation. See <see cref="FetchPreflistOptions"/></param>
        public FetchPreflist(FetchPreflistOptions options)
            : base(options)
        {
        }

        public override MessageCode ExpectedCode
        {
            get { throw new System.NotImplementedException(); }
        }

        public override RpbReq ConstructPbRequest()
        {
            throw new System.NotImplementedException();
        }

        public override void OnSuccess(RpbResp response)
        {
            if (response == null)
            {
                Response = new PreflistResponse();
            }
            else
            {
                RpbGetBucketKeyPreflistResp resp = (RpbGetBucketKeyPreflistResp)response;

                IEnumerable<PreflistItem> preflistItems = Enumerable.Empty<PreflistItem>();

                if (EnumerableUtil.NotNullOrEmpty(resp.preflist))
                {
                    preflistItems = resp.preflist.Select(i => new PreflistItem());
                }

                Response = new PreflistResponse(Options.Key, preflistItems);
            }
        }

        /// <inheritdoc />
        public class Builder
            : CommandBuilder<FetchPreflist.Builder, FetchPreflist, FetchPreflistOptions>
        {
            public override FetchPreflist Build()
            {
                throw new System.NotImplementedException();
            }
        }
    }
}