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

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RiakClient.Models.MapReduce.Inputs
{
    public class RiakBucketKeyKeyDataInput : RiakPhaseInput
    {
        private class RiakBucketKeyKeyDataInputItem
        {
            internal RiakObjectId ObjectId { get; private set; }
            internal Object KeyData { get; private set; }

            internal RiakBucketKeyKeyDataInputItem(RiakObjectId objectId, object keyData)
            {
                ObjectId = objectId;
                KeyData = keyData;
            }

            // Helper constructor
            internal RiakBucketKeyKeyDataInputItem(Tuple<RiakObjectId, object> data) : this(data.Item1, data.Item2)
            {
            }

            // Helper constructor for old-style RiakBucketKeyKeyDataInput builder methods.
            // Will be removed in future.
            internal RiakBucketKeyKeyDataInputItem(Tuple<string, string, object> data)
                : this(new RiakObjectId(data.Item1, data.Item2), data.Item3)
            {
            }
        }

        private List<RiakBucketKeyKeyDataInputItem> BucketKeyKeyData { get; set; }

        public RiakBucketKeyKeyDataInput()
        {
            BucketKeyKeyData = new List<RiakBucketKeyKeyDataInputItem>();
        }

        [Obsolete("Use the Add() that accepts a RiakIndexId instead. This will be removed in the next version.")]
        public RiakBucketKeyKeyDataInput Add(string bucket, string key, object keyData)
        {
            BucketKeyKeyData.Add(new RiakBucketKeyKeyDataInputItem(new RiakObjectId(bucket, key), keyData));
            return this;
        }

        [Obsolete("Use the Add() that accepts a RiakIndexId instead. This will be removed in the next version.")]
        public RiakBucketKeyKeyDataInput Add(params Tuple<string, string, object>[] pairs)
        {
            BucketKeyKeyData.AddRange(pairs.Select(p => new RiakBucketKeyKeyDataInputItem(p)));
            return this;
        }

        [Obsolete("Use the Add() that accepts a RiakIndexId instead. This will be removed in the next version.")]
        public RiakBucketKeyKeyDataInput Add(IEnumerable<Tuple<string, string, object>> pairs)
        {
            BucketKeyKeyData.AddRange(pairs.Select(p => new RiakBucketKeyKeyDataInputItem(p)));
            return this;
        }

        public RiakBucketKeyKeyDataInput Add(RiakObjectId objectId, object keyData)
        {
            BucketKeyKeyData.Add(new RiakBucketKeyKeyDataInputItem(objectId, keyData));
            return this;
        }

        public RiakBucketKeyKeyDataInput Add(params Tuple<RiakObjectId, object>[] pairs)
        {
            AddRange(pairs);
            return this;
        }

        public RiakBucketKeyKeyDataInput Add(IEnumerable<Tuple<RiakObjectId, object>> pairs)
        {
            AddRange(pairs);
            return this;
        }

        private void AddRange(IEnumerable<Tuple<RiakObjectId, object>> pairs)
        {
            BucketKeyKeyData.AddRange(pairs.Select(p => new RiakBucketKeyKeyDataInputItem(p)));
        }
        
        public override JsonWriter WriteJson(JsonWriter writer)
        {
            writer.WritePropertyName("inputs");
            writer.WriteStartArray();

            var s = new JsonSerializer();

            foreach (var bkkd in BucketKeyKeyData)
            {
                writer.WriteStartArray();
                writer.WriteValue(bkkd.ObjectId.Bucket);
                writer.WriteValue(bkkd.ObjectId.Key);
                s.Serialize(writer, bkkd.KeyData);
                if (bkkd.ObjectId.BucketType != null)
                {
                    writer.WriteValue(bkkd.ObjectId.BucketType);
                }
                writer.WriteEndArray();
            }

            writer.WriteEndArray();

            return writer;
        }
    }
}
