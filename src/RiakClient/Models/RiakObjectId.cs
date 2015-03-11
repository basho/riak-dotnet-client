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

    /// <summary>
    /// An Id that specifies where an object lives in Riak. Immutable once created. 
    /// </summary>
    [JsonConverter(typeof(RiakObjectIdConverter))]
    public class RiakObjectId : IEquatable<RiakObjectId>
    {
        private readonly string bucket;
        private readonly string bucketType;
        private readonly string key;

        /// <summary>
        /// Initializes a new instance of the <see cref="RiakObjectId" /> class.
        /// Build an Id using the default (pre-2.0) Bucket Type, and the specified Bucket and Key.
        /// </summary>
        /// <param name="bucket">The bucket name to use.</param>
        /// <param name="key">The key to use.</param>
        /// <exception cref="ArgumentOutOfRangeException">The value of 'bucket' cannot be null, an empty string, or whitespace.</exception>
        public RiakObjectId(string bucket, string key)
        {
            if (string.IsNullOrWhiteSpace(bucket))
            {
                throw new ArgumentOutOfRangeException("bucket");
            }

            this.bucket = bucket;
            this.key = key;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RiakObjectId" /> class.
        /// Build an Id using the specified Bucket Type, Bucket, and Key.
        /// </summary>
        /// <param name="bucketType">The bucket type to use.</param>
        /// <param name="bucket">The bucket name to use.</param>
        /// <param name="key">The key to use.</param>
        public RiakObjectId(string bucketType, string bucket, string key)
            : this(bucket, key)
        {
            this.bucketType = bucketType;
        }

        /// <summary>
        /// Get the bucket name.
        /// </summary>
        public string Bucket
        {
            get { return bucket; }
        }

        /// <summary>
        /// Get the bucket type.
        /// </summary>
        public string BucketType
        {
            get { return bucketType; }
        }

        /// <summary>
        /// Get the key.
        /// </summary>
        public string Key
        {
            get { return key; }
        }

        /// <summary>
        /// Determines whether the one object is equal to another object.
        /// </summary>
        /// <param name="left">The first <see cref="RiakObjectId"/> to compare.</param>
        /// <param name="right">The other <see cref="RiakObjectId"/> to compare.</param>
        /// <returns><b>true</b> if the specified object is equal to the current object, otherwise, <b>false</b>.</returns>
        public static bool operator ==(RiakObjectId left, RiakObjectId right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Determines whether the one object is <b>not</b> equal to another object.
        /// </summary>
        /// <param name="left">The first <see cref="RiakObjectId"/> to compare.</param>
        /// <param name="right">The other <see cref="RiakObjectId"/> to compare.</param>
        /// <returns><b>true</b> if the specified object is <b>not</b> equal to the current object, otherwise, <b>false</b>.</returns>
        public static bool operator !=(RiakObjectId left, RiakObjectId right)
        {
            return !Equals(left, right);
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="other">The object to compare with the current object.</param>
        /// <returns><b>true</b> if the specified object is equal to the current object, otherwise, <b>false</b>.</returns>
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

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns><b>true</b> if the specified object is equal to the current object, otherwise, <b>false</b>.</returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as RiakObjectId);
        }

        /// <summary>
        /// Returns a hash code for the current object.
        /// Uses a combination of the public properties to generate a unique hash code.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
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

        /// <summary>
        /// Returns a string that represents the RiakObjectId value.
        /// </summary>
        /// <returns>A string representation of the Bucket Type, Bucket Name, and Key.</returns>
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
