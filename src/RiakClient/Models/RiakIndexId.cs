// <copyright file="RiakIndexId.cs" company="Basho Technologies, Inc.">
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

    /// <summary>
    /// An Id that specifies a specific index in Riak. Immutable once created. 
    /// </summary>
    public class RiakIndexId : IEquatable<RiakIndexId>
    {
        private readonly string bucketName;
        private readonly string bucketType;
        private readonly string indexName;

        /// <summary>
        /// Initializes a new instance of the <see cref="RiakIndexId" /> class.
        /// </summary>
        /// <param name="bucketName">The bucket name to use.</param>
        /// <param name="indexName">The index name to use.</param>
        public RiakIndexId(string bucketName, string indexName)
            : this(null, bucketName, indexName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RiakIndexId" /> class.
        /// </summary>
        /// <param name="bucketType">The bucket type to use.</param>
        /// <param name="bucketName">The bucket name to use.</param>
        /// <param name="indexName">The index name to use.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="bucketName"/> cannot be null, empty, or whitespace.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="indexName"/> cannot be null, empty, or whitespace.</exception>
        public RiakIndexId(string bucketType, string bucketName, string indexName)
        {
            if (string.IsNullOrEmpty(bucketName))
            {
                throw new ArgumentOutOfRangeException("bucketName", "bucketName cannot be null, empty, or whitespace.");
            }

            if (string.IsNullOrEmpty(indexName))
            {
                throw new ArgumentOutOfRangeException("indexName", "indexName cannot be null, empty, or whitespace.");
            }

            this.bucketType = bucketType;
            this.bucketName = bucketName;
            this.indexName = indexName;
        }

        /// <summary>
        /// Get the Bucket Type of the Index Id.
        /// </summary>
        public string BucketType
        {
            get { return bucketType; }
        }

        /// <summary>
        /// Get the Bucket Name of the Index Id.
        /// </summary>
        public string BucketName
        {
            get { return bucketName; }
        }

        /// <summary>
        /// Get the Index Name of the Index Id.
        /// </summary>
        public string IndexName
        {
            get { return indexName; }
        }

        /// <summary>
        /// Determines whether the one object is equal to another object.
        /// </summary>
        /// <param name="left">The first <see cref="RiakIndexId"/> to compare.</param>
        /// <param name="right">The other <see cref="RiakIndexId"/> to compare.</param>
        /// <returns><b>true</b> if the specified object is equal to the current object, otherwise, <b>false</b>.</returns>
        public static bool operator ==(RiakIndexId left, RiakIndexId right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Determines whether the one object is <b>not</b> equal to another object.
        /// </summary>
        /// <param name="left">The first <see cref="RiakObjectId"/> to compare.</param>
        /// <param name="right">The other <see cref="RiakObjectId"/> to compare.</param>
        /// <returns><b>true</b> if the specified object is <b>not</b> equal to the current object, otherwise, <b>false</b>.</returns>
        public static bool operator !=(RiakIndexId left, RiakIndexId right)
        {
            return !Equals(left, right);
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="other">The object to compare with the current object.</param>
        /// <returns><b>true</b> if the specified object is equal to the current object, otherwise, <b>false</b>.</returns>
        public bool Equals(RiakIndexId other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return string.Equals(bucketName, other.bucketName) &&
                   string.Equals(bucketType, other.bucketType) &&
                   string.Equals(indexName, other.indexName);
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns><b>true</b> if the specified object is equal to the current object, otherwise, <b>false</b>.</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            return Equals(obj as RiakIndexId);
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
                var hashCode = bucketName != null ? bucketName.GetHashCode() : 0;
                hashCode = (hashCode * 397) ^ (bucketType != null ? bucketType.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (indexName != null ? indexName.GetHashCode() : 0);
                return hashCode;
            }
        }

        internal RiakBinIndexId ToBinIndexId()
        {
            return new RiakBinIndexId(BucketType, BucketName, IndexName);
        }

        internal RiakIntIndexId ToIntIndexId()
        {
            return new RiakIntIndexId(BucketType, BucketName, IndexName);
        }
    }
}
