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

using System;
using RiakClient.Extensions;

namespace RiakClient.Models
{
    public class RiakIndexId : IEquatable<RiakIndexId>
    {
        private readonly string bucketName;
        private readonly string bucketType;
        private readonly string indexName;

        public RiakIndexId(string bucketName, string indexName)
            : this(null, bucketName, indexName)
        {
        }

        public RiakIndexId(string bucketType, string bucketName, string indexName)
        {
            if (String.IsNullOrEmpty(bucketName))
            {
                throw new ArgumentOutOfRangeException("bucketName", "bucketName cannot be null, empty, or whitespace.");
            }

            if (String.IsNullOrEmpty(indexName))
            {
                throw new ArgumentOutOfRangeException("indexName", "indexName cannot be null, empty, or whitespace.");
            }

            this.bucketType = bucketType;
            this.bucketName = bucketName;
            this.indexName = indexName;
        }

        public string BucketType
        {
            get { return bucketType; }
        }

        public string BucketName
        {
            get { return bucketName; }
        }

        public string IndexName
        {
            get { return indexName; }
        }

        internal RiakBinIndexId ToBinIndexId()
        {
            return new RiakBinIndexId(BucketType, BucketName, IndexName);
        }

        internal RiakIntIndexId ToIntIndexId()
        {
            return new RiakIntIndexId(BucketType, BucketName, IndexName);
        }

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

            return String.Equals(bucketName, other.bucketName) &&
                   String.Equals(bucketType, other.bucketType) &&
                   String.Equals(indexName, other.indexName);
        }

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

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (bucketName != null ? bucketName.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (bucketType != null ? bucketType.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (indexName != null ? indexName.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static bool operator ==(RiakIndexId left, RiakIndexId right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(RiakIndexId left, RiakIndexId right)
        {
            return !Equals(left, right);
        }
    }

    internal class RiakBinIndexId : RiakIndexId
    {
        public RiakBinIndexId(string bucketName, string indexName)
            : base(bucketName, indexName.ToBinaryKey())
        {
        }

        public RiakBinIndexId(string bucketType, string bucketName, string indexName)
            : base(bucketType, bucketName, indexName.ToBinaryKey())
        {
        }
    }

    internal class RiakIntIndexId : RiakIndexId
    {
        public RiakIntIndexId(string bucketName, string indexName)
            : base(bucketName, indexName.ToIntegerKey())
        {
        }

        public RiakIntIndexId(string bucketType, string bucketName, string indexName)
            : base(bucketType, bucketName, indexName.ToIntegerKey())
        {
        }
    }
}
