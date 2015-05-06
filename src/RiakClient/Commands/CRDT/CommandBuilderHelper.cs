// <copyright file="CommandBuilderHelper.cs" company="Basho Technologies, Inc.">
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

    /// <summary>
    /// Helps build a Riak command.
    /// </summary>
    internal class CommandBuilderHelper
    {
        private string bucketType;
        private string bucket;
        private string key;
        private TimeSpan timeout;

        public string BucketType
        {
            get { return bucketType; }
        }

        public string Bucket
        {
            get { return bucket; }
        }

        public string Key
        {
            get { return key; }
        }

        public TimeSpan Timeout
        {
            get { return timeout; }
        }

        public void WithBucketType(string bucketType)
        {
            if (string.IsNullOrWhiteSpace(bucketType))
            {
                throw new ArgumentNullException("bucketType", "bucketType may not be null, empty or whitespace");
            }

            this.bucketType = bucketType;
        }

        public void WithBucket(string bucket)
        {
            if (string.IsNullOrWhiteSpace(bucket))
            {
                throw new ArgumentNullException("bucket", "bucket may not be null, empty or whitespace");
            }

            this.bucket = bucket;
        }

        public void WithKey(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException("key", "key may not be null, empty or whitespace");
            }

            this.key = key;
        }

        public void WithTimeout(TimeSpan timeout)
        {
            this.timeout = timeout;
        }
    }
}