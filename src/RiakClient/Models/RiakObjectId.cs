// <copyright file="RiakObjectId.cs" company="Basho Technologies, Inc.">
// Copyright (c) 2011 - OJ Reeves & Jeremiah Peschka
// Copyright (c) 2014 - Basho Technologies, Inc.
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

namespace RiakClient.Models
{
    using System;
    using Converters;
    using Newtonsoft.Json;
    using Util;

    [JsonConverter(typeof(RiakObjectIdConverter))]
    public class RiakObjectId : IEquatable<RiakObjectId>
    {
        private readonly string bucket;
        private readonly string bucketType;
        private readonly string key;

        public RiakObjectId(string bucket, string key)
        {
            if (EnumerableUtil.IsNullOrEmpty(bucket))
            {
                throw new ArgumentNullException("bucket");
            }

            this.bucket = bucket;
            this.key = key;
        }

        public RiakObjectId(string bucketType, string bucket, string key)
            : this(bucket, key)
        {
            this.bucketType = bucketType;
        }

        public string Bucket
        {
            get { return bucket; }
        }

        public string BucketType
        {
            get { return bucketType; }
        }

        public string Key
        {
            get { return key; }
        }

        public static bool operator ==(RiakObjectId left, RiakObjectId right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(RiakObjectId left, RiakObjectId right)
        {
            return !Equals(left, right);
        }

        public bool Equals(RiakObjectId other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return string.Equals(bucket, other.Bucket) &&
                string.Equals(bucketType, other.BucketType) &&
                string.Equals(key, other.key);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as RiakObjectId);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = bucket != null ? bucket.GetHashCode() : 0;
                hashCode = (hashCode * 397) ^ (bucketType != null ? bucketType.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (key != null ? key.GetHashCode() : 0);
                return hashCode;
            }
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(BucketType))
            {
                return string.Format("BucketType: DefaultBucketType, Bucket: {0}, Key: {1}", Bucket, Key);
            }

            return string.Format("BucketType: {0}, Bucket: {1}, Key: {2}", BucketType, Bucket, Key);
        }

        internal RiakLink ToRiakLink(string tag)
        {
            return new RiakLink(bucket, key, tag);
        }
    }
}
