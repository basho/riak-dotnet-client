// <copyright file="UpdateMap.cs" company="Basho Technologies, Inc.">
// Copyright (c) 2015 - Basho Technologies, Inc.
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
    using System.Collections;
    using System.Collections.Generic;
    using Messages;
    using Models;
    using Util;

    public class UpdateMap
    {
        private readonly UpdateMapOptions options;

        public UpdateMap(UpdateMapOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException("options");
            }

            this.options = options;
        }

        internal DtUpdateReq ConstructPbRequest()
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

            return req;
        }

        public class MapOperation
        {
            private readonly CounterOperations incrementCounters = new CounterOperations();
            private readonly CounterOperations removeCounters = new CounterOperations();

            public void IncrementCounter(string key, int increment)
            {
                RemoveRemovesFor(key, removeCounters);
                incrementCounters.Increment(key, increment);
            }

            private static void RemoveRemovesFor(string key, IDictionary ops)
            {
                if (ops.Contains(key))
                {
                    ops.Remove(key);
                }
            }

            private class CounterOperations : Dictionary<string, int>
            {
                public void Increment(string key, int increment)
                {
                    if (this.ContainsKey(key))
                    {
                        this[key] += increment;
                    }
                    else
                    {
                        this[key] = increment;
                    }
                }
            }
        }

        public class Builder
        {
            private string bucketType;
            private string bucket;
            private string key;

            private MapOperation mapOp;
            private byte[] context;

            private Quorum w;
            private Quorum pw;
            private Quorum dw;

            private bool returnBody;
            private bool includeContext;

            private TimeSpan timeout;

            public UpdateMap Build()
            {
                var options = new UpdateMapOptions(bucketType, bucket, key);

                options.W = w;
                options.PW = pw;
                options.DW = dw;

                options.ReturnBody = returnBody;

                options.Timeout = timeout;

                options.Context = context;

                return new UpdateMap(options);
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

            public Builder WithMapOperation(MapOperation mapOp)
            {
                if (mapOp == null)
                {
                    throw new ArgumentNullException("mapOp", "mapOp may not be null");
                }

                this.mapOp = mapOp;
                return this;
            }

            public Builder WithContext(byte[] context)
            {
                if (EnumerableUtil.IsNullOrEmpty(context))
                {
                    throw new ArgumentNullException("context", "context may not be null or empty");
                }

                this.context = context;
                return this;
            }

            public Builder WithW(Quorum w)
            {
                if (w == null)
                {
                    throw new ArgumentNullException("w", "w may not be null");
                }

                this.w = w;
                return this;
            }

            public Builder WithPW(Quorum pw)
            {
                if (pw == null)
                {
                    throw new ArgumentNullException("pw", "pw may not be null");
                }

                this.pw = pw;
                return this;
            }

            public Builder WithDW(Quorum dw)
            {
                if (dw == null)
                {
                    throw new ArgumentNullException("dw", "dw may not be null");
                }

                this.dw = dw;
                return this;
            }

            public Builder WithReturnBody(bool returnBody)
            {
                this.returnBody = returnBody;
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