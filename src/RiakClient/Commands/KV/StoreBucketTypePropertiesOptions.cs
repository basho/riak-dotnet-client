// <copyright file="StoreBucketTypePropertiesOptions.cs" company="Basho Technologies, Inc.">
// Copyright 2017 - Basho Technologies, Inc.
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

namespace RiakClient.Commands.KV
{
    using System;
    using Models;

    /// <summary>
    /// Represents options for a <see cref="FetchBucketTypeProperties"/> operation.
    /// </summary>
    /// <inheritdoc/>
    public class StoreBucketTypePropertiesOptions : CommandOptions, IEquatable<StoreBucketTypePropertiesOptions>
    {
        private readonly string bucketType;
        private readonly RiakBucketProperties bucketProperties;

        public StoreBucketTypePropertiesOptions(string bucketType, RiakBucketProperties bucketProperties)
        {
            if (string.IsNullOrEmpty(bucketType))
            {
                throw new ArgumentNullException(nameof(bucketType));
            }

            if (bucketProperties == null)
            {
                throw new ArgumentNullException(nameof(bucketProperties));
            }

            this.bucketType = bucketType;
            this.bucketProperties = bucketProperties;
        }

        /// <summary>
        /// The Bucket Type to assign the properties to.
        /// </summary>
        /// <value>A <see cref="RiakString"/> representing the bucket type.</value>
        public RiakString BucketType
        {
            get { return bucketType; }
        }

        /// <summary>
        /// The Bucket Properties to store.
        /// </summary>
        /// <value>A <see cref="RiakBucketProperties"/> representing the properties to assign to the bucket type.</value>
        public RiakBucketProperties BucketProperties
        {
            get { return bucketProperties; }
        }

        public bool Equals(StoreBucketTypePropertiesOptions other)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return this.GetHashCode() == other.GetHashCode();
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = base.GetHashCode();
                hashCode = (hashCode * 397) ^ bucketType.GetHashCode();
                hashCode = (hashCode * 397) ^ bucketProperties.GetHashCode();
                return hashCode;
            }
        }
    }
}