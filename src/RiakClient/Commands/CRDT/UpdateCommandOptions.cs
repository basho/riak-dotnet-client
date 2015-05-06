// <copyright file="UpdateCommandOptions.cs" company="Basho Technologies, Inc.">
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
    /// Represents options for a <see cref="UpdateCommand{TResponse}"/> operation.
    /// </summary>
    public abstract class UpdateCommandOptions
    {
        private readonly RiakString bucketType;
        private readonly RiakString bucket;
        private readonly RiakString key;

        private bool returnBody = true;
        private bool includeContext = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateCommandOptions"/> class.
        /// </summary>
        /// <param name="bucketType">The bucket type in Riak. Required.</param>
        /// <param name="bucket">The bucket in Riak. Required.</param>
        /// <param name="key">The key in Riak. If <b>null</b>, Riak will generate a key.</param>
        public UpdateCommandOptions(string bucketType, string bucket, string key)
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

            this.key = key;
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
        /// The key for the map you want to store. If not supplied Riak will generate one.
        /// </summary>
        public RiakString Key
        {
            get { return key; }
        }

        /// <summary>
        /// The W (write) value to use.
        /// </summary>
        public Quorum W { get; set; }

        /// <summary>
        /// The PW (primary vnode write) value to use.
        /// </summary>
        public Quorum PW { get; set; }

        /// <summary>
        /// The DW (durable write) value to use.
        /// </summary>
        public Quorum DW { get; set; }

        /// <summary>
        /// If true, returns the updated CRDT.
        /// </summary>
        public bool ReturnBody
        {
            get { return returnBody; }
            set { returnBody = value; }
        }

        /// <summary>
        /// The timeout for this command.
        /// </summary>
        public TimeSpan Timeout { get; set; }

        /// <summary>
        /// The context from a previous fetch. Required for remove operations. 
        /// </summary>
        public byte[] Context { get; set; }

        /// <summary>
        /// Set to <b>false</b> to not return context. Default (and recommended value) is <b>true</b>.
        /// </summary>
        public bool IncludeContext
        {
            get { return includeContext; }
            set { includeContext = value; }
        }

        /// <summary>
        /// Returns to <b>true</b> if this command has removals.
        /// </summary>
        public bool HasRemoves
        {
            get { return GetHasRemoves(); }
        }

        protected abstract bool GetHasRemoves();
    }
}