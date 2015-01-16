// Copyright (c) 2011 - OJ Reeves & Jeremiah Peschka
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

using System;
using CorrugatedIron.Converters;
using CorrugatedIron.Extensions;
using Newtonsoft.Json;

namespace CorrugatedIron.Models
{
    [JsonConverter(typeof(RiakObjectIdConverter))]
    public class RiakObjectId : IEquatable<RiakObjectId>
    {
        protected RiakObjectId()
        {
        }

        public RiakObjectId(string bucket, string key)
        {
            if (bucket.IsNullOrEmpty())
            {
                throw new ArgumentNullException("bucket");
            }

            this.Bucket = bucket;
            this.Key = key;
        }

        public RiakObjectId(string bucketType, string bucket, string key)
            : this(bucket, key)
        {
            this.BucketType = bucketType;
        }

        internal RiakLink ToRiakLink(string tag)
        {
            return new RiakLink(Bucket, Key, tag);
        }

        public string Bucket { get; private set; }
        public string BucketType { get; private set; }
        public string Key { get; private set; }

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
            return String.Equals(Bucket, other.Bucket) &&
                String.Equals(BucketType, other.BucketType) &&
                String.Equals(Key, other.Key);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as RiakObjectId);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (Bucket != null ? Bucket.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (BucketType != null ? BucketType.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Key != null ? Key.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static bool operator ==(RiakObjectId left, RiakObjectId right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(RiakObjectId left, RiakObjectId right)
        {
            return !Equals(left, right);
        }
    }
}
