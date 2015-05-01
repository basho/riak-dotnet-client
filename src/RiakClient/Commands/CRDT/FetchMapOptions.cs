// <copyright file="FetchMapOptions.cs" company="Basho Technologies, Inc.">
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
    /// Represents options for a <see cref="FetchMap"/> operation.
    /// </summary>
    public class FetchMapOptions
    {
        private readonly RiakString bucketType;
        private readonly RiakString bucket;
        private readonly RiakString key;

        /// <summary>
        /// Initializes a new instance of the <see cref="FetchMapOptions"/> class.
        /// </summary>
        /// <param name="bucketType">The bucket type in Riak. Required.</param>
        /// <param name="bucket">The bucket in Riak. Required.</param>
        /// <param name="key">The key in Riak. Required.</param>
        public FetchMapOptions(string bucketType, string bucket, string key)
        {
            if (string.IsNullOrEmpty(bucketType))
            {
                throw new ArgumentNullException("bucketType");
            }
            else
            {
                this.bucketType = bucketType;
            }

            if (string.IsNullOrEmpty(bucket))
            {
                throw new ArgumentNullException("bucket");
            }
            else
            {
                this.bucket = bucket;
            }

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }
            else
            {
                this.key = key;
            }

            // ensure default values
            this.NotFoundOk = false;
            this.UseBasicQuorum = false;
            this.IncludeContext = true;
        }

        /// <summary>
        /// The bucket type
        /// </summary>
        /// <value>A <see cref="RiakString"/> representing the bucket type.</value>
        public RiakString BucketType
        {
            get { return bucketType; }
        }

        /// <summary>
        /// The bucket
        /// </summary>
        /// <value>A <see cref="RiakString"/> representing the bucket.</value>
        public RiakString Bucket
        {
            get { return bucket; }
        }

        /// <summary>
        /// The key
        /// </summary>
        /// <value>The <see cref="RiakString"/> representing the key.</value>
        public RiakString Key
        {
            get { return key; }
        }

        /// <summary>
        /// The timeout for this command.
        /// </summary>
        public TimeSpan Timeout { get; set; }

        /// <summary>
        /// The R (read) value to use.
        /// </summary>
        public Quorum R { get; set; }

        /// <summary>
        /// The PR (primary vnode read) value to use.
        /// </summary>
        public Quorum PR { get; set; }

        /// <summary>
        /// If true, a <c>not_found</c> response from Riak is not an error.
        /// </summary>
        public bool NotFoundOk { get; set; }

        /// <summary>
        /// Controls whether a read request should return early in some failure cases.
        /// </summary>
        public bool UseBasicQuorum { get; set; }

        /// <summary>
        /// Set to <b>false</b> to not return context. Default (and recommended value) is <b>true</b>.
        /// </summary>
        public bool IncludeContext { get; set; }
    }
}