// <copyright file="FetchBucketTypePropertiesOptions.cs" company="Basho Technologies, Inc.">
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

    /// <summary>
    /// Represents options for a <see cref="FetchBucketTypeProperties"/> operation.
    /// </summary>
    /// <inheritdoc/>
    public class FetchBucketTypePropertiesOptions : CommandOptions, IEquatable<FetchBucketTypePropertiesOptions>
    {
        private readonly string bucketType;

        public FetchBucketTypePropertiesOptions(string bucketType)
        {
            if (string.IsNullOrEmpty(bucketType))
            {
                throw new ArgumentNullException("bucketType");
            }
            else
            {
                this.bucketType = bucketType;
            }
        }

        /// <summary>
        /// The bucket type
        /// </summary>
        /// <value>A <see cref="RiakString"/> representing the bucket type.</value>
        public RiakString BucketType
        {
            get { return bucketType; }
        }

        public bool Equals(FetchBucketTypePropertiesOptions other)
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

        public override bool Equals(object obj)
        {
            return Equals(obj as FetchBucketTypePropertiesOptions);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (base.GetHashCode() * 397) ^ bucketType.GetHashCode();
            }
        }
    }
}