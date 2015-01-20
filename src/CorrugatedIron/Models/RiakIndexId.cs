// Copyright (c) 2011 - 2014 OJ Reeves & Jeremiah Peschka
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

using System;
using CorrugatedIron.Extensions;

namespace CorrugatedIron.Models
{
    public class RiakIndexId : IEquatable<RiakIndexId>
    {
        private readonly string _bucketName;
        private readonly string _bucketType;
        private readonly string _indexName;

        public RiakIndexId(string bucketName, string indexName) : this(null, bucketName, indexName)
        {
        }

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

            _bucketType = bucketType;
            _bucketName = bucketName;
            _indexName = indexName;
        }

        public string BucketType
        {
            get { return _bucketType; }
        }

        public string BucketName
        {
            get { return _bucketName; }
        }

        public string IndexName
        {
            get { return _indexName; }
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
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(_bucketName, other._bucketName) && string.Equals(_bucketType, other._bucketType) && string.Equals(_indexName, other._indexName);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((RiakIndexId) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (_bucketName != null ? _bucketName.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (_bucketType != null ? _bucketType.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (_indexName != null ? _indexName.GetHashCode() : 0);
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