// <copyright file="UpdateMapOptions.cs" company="Basho Technologies, Inc.">
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
    using Models;

    public class UpdateMapOptions
    {
        private readonly RiakString bucketType;
        private readonly RiakString bucket;
        private readonly UpdateMap.MapOperation op;

        public UpdateMapOptions(string bucketType, string bucket, UpdateMap.MapOperation op)
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

            if (op == null)
            {
                throw new ArgumentNullException("op");
            }
            else
            {
                this.op = op;
            }
        }

        public RiakString BucketType
        {
            get { return bucketType; }
        }

        public RiakString Bucket
        {
            get { return bucket; }
        }

        public UpdateMap.MapOperation Op
        {
            get { return op; }
        }

        public RiakString Key { get; set; }

        public Quorum W { get; set; }

        public Quorum PW { get; set; }

        public Quorum DW { get; set; }

        public bool ReturnBody { get; set; }

        public TimeSpan Timeout { get; set; }

        public byte[] Context { get; set; }

        public bool IncludeContext { get; set; }
    }
}