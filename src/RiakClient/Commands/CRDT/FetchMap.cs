// <copyright file="FetchMap.cs" company="Basho Technologies, Inc.">
// Copyright © 2015 - Basho Technologies, Inc.
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
    public class FetchMap : IRiakCommand
    {
        private readonly FetchMapOptions options;

        /// <summary>
        /// Initializes a new instance of the <see cref="FetchMap"/> class.
        /// </summary>
        /// <param name="options">Options for this operation. See <see cref="FetchMapOptions"/></param>
        public FetchMap(FetchMapOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException("options");
            }

            this.options = options;
        }

        public MessageCode ExpectedCode
        {
            get { return MessageCode.DtFetchResp; }
        }

        public MapResponse Response { get; private set; }

        public RpbReq ConstructPbRequest()
        {
            var req = new DtFetchReq();

            req.type = options.BucketType;
            req.bucket = options.Bucket;
            req.key = options.Key;

            req.r = options.R;
            req.pr = options.PR;

            req.timeout = (uint)options.Timeout.TotalMilliseconds;
            req.include_context = options.IncludeContext;

            return req;
        }

        public void OnSuccess(RpbResp response)
        {
            throw new NotImplementedException();
        }

        public class Builder
        {
            private string bucketType;
            private string bucket;
            private string key;

            private Quorum r;
            private Quorum pr;

            private TimeSpan timeout;
            private bool includeContext;

            public FetchMap Build()
            {
                var options = new FetchMapOptions(bucketType, bucket, key);

                options.R = r;
                options.PR = pr;

                options.Timeout = timeout;
                options.IncludeContext = includeContext;

                return new FetchMap(options);
            }

            public Builder WithBucketType(string bucketType)
            {
                if (string.IsNullOrWhiteSpace(bucketType))
                {
                    throw new ArgumentNullException("bucketType", "bucketType may not be null, empty or whitespace");
                }

                this.bucketType = bucketType;
                return this;
            }

            public Builder WithBucket(string bucket)
            {
                if (string.IsNullOrWhiteSpace(bucket))
                {
                    throw new ArgumentNullException("bucket", "bucket may not be null, empty or whitespace");
                }

                this.bucket = bucket;
                return this;
            }

            public Builder WithKey(string key)
            {
                if (string.IsNullOrWhiteSpace(key))
                {
                    throw new ArgumentNullException("key", "key may not be null, empty or whitespace");
                }

                this.key = key;
                return this;
            }

            public Builder WithR(Quorum r)
            {
                if (r == null)
                {
                    throw new ArgumentNullException("r", "r may not be null");
                }

                this.r = r;
                return this;
            }

            public Builder WithPR(Quorum pr)
            {
                if (pr == null)
                {
                    throw new ArgumentNullException("pr", "pr may not be null");
                }

                this.pr = pr;
                return this;
            }

            public Builder WithIncludeContext(bool includeContext)
            {
                this.includeContext = includeContext;
                return this;
            }

            public Builder WithTimeout(TimeSpan timeout)
            {
                this.timeout = timeout;
                return this;
            }
        }
    }
}