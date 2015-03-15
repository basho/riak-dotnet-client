// <copyright file="RiakBucketKeyKeyDataInput.cs" company="Basho Technologies, Inc.">
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

namespace RiakClient.Models.MapReduce.Inputs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Newtonsoft.Json;

    /// <summary>
    /// Represents a Bucket/Key/KeyData collection based mapreduce input.
    /// </summary>
    public class RiakBucketKeyKeyDataInput : RiakPhaseInput
    {
        private readonly List<RiakBucketKeyKeyDataInputItem> bucketKeyKeyData = new List<RiakBucketKeyKeyDataInputItem>();

        /// <summary>
        /// Adds a (bucket, key, keyData) tuple to the mapreduce inputs collection.
        /// </summary>
        /// <param name="bucket">The bucket of the object address to add to the inputs collection.</param>
        /// <param name="key">The key of the object address to add to the inputs collection.</param>
        /// <param name="keyData">
        /// The keyData that goes with the <paramref name="bucket"/> and 
        /// <paramref name="key"/> to add to the inputs collection. 
        /// </param>
        /// <returns>A reference to this updated instance, for fluent chaining.</returns>
        [Obsolete("Use the Add() that accepts a RiakIndexId instead. This will be removed in the next version.")]
        public RiakBucketKeyKeyDataInput Add(string bucket, string key, object keyData)
        {
            bucketKeyKeyData.Add(new RiakBucketKeyKeyDataInputItem(new RiakObjectId(bucket, key), keyData));
            return this;
        }

        /// <summary>
        /// Adds a collection of (bucket, key, keyData) tuples to the mapreduce inputs collection.
        /// </summary>
        /// <param name="pairs">
        /// The (bucket, key, keyData) tuple params array to add to the inputs collection.
        /// </param>
        /// <returns>A reference to this updated instance, for fluent chaining.</returns>
        [Obsolete("Use the Add() that accepts a RiakIndexId instead. This will be removed in the next version.")]
        public RiakBucketKeyKeyDataInput Add(params Tuple<string, string, object>[] pairs)
        {
            bucketKeyKeyData.AddRange(pairs.Select(p => new RiakBucketKeyKeyDataInputItem(p)));
            return this;
        }

        /// <summary>
        /// Adds a collection of (bucket, key, keyData) tuples to the mapreduce inputs collection.
        /// </summary>
        /// <param name="pairs">
        /// The (bucket, key, keyData) tuple collection to add to the inputs collection.
        /// </param>
        /// <returns>A reference to this updated instance, for fluent chaining.</returns>
        [Obsolete("Use the Add() that accepts a RiakIndexId instead. This will be removed in the next version.")]
        public RiakBucketKeyKeyDataInput Add(IEnumerable<Tuple<string, string, object>> pairs)
        {
            bucketKeyKeyData.AddRange(pairs.Select(p => new RiakBucketKeyKeyDataInputItem(p)));
            return this;
        }

        /// <summary>
        /// Adds a (<see cref="RiakObjectId"/>, keyData) pair to the mapreduce inputs collection.
        /// </summary>
        /// <param name="objectId">The <see cref="RiakObjectId"/> to add to the inputs collection.</param>
        /// <param name="keyData">
        /// The keyData that goes with the <paramref name="objectId"/> to add to the inputs collection.
        /// </param>
        /// <returns>A reference to this updated instance, for fluent chaining.</returns>
        public RiakBucketKeyKeyDataInput Add(RiakObjectId objectId, object keyData)
        {
            bucketKeyKeyData.Add(new RiakBucketKeyKeyDataInputItem(objectId, keyData));
            return this;
        }

        /// <summary>
        /// Adds a collection of (<see cref="RiakObjectId"/>, keyData) tuples to the mapreduce inputs collection.
        /// </summary>
        /// <param name="pairs">
        /// The (<see cref="RiakObjectId"/>, keyData) tuple params array to add to the inputs collection.
        /// </param>
        /// <returns>A reference to this updated instance, for fluent chaining.</returns>
        public RiakBucketKeyKeyDataInput Add(params Tuple<RiakObjectId, object>[] pairs)
        {
            AddRange(pairs);
            return this;
        }

        /// <summary>
        /// Adds a collection of (<see cref="RiakObjectId"/>, keyData) tuples to the mapreduce inputs collection.
        /// </summary>
        /// <param name="pairs">
        /// The (<see cref="RiakObjectId"/>, keyData) tuple collection to add to the inputs collection.
        /// </param>
        /// <returns>A reference to this updated instance, for fluent chaining.</returns>
        public RiakBucketKeyKeyDataInput Add(IEnumerable<Tuple<RiakObjectId, object>> pairs)
        {
            AddRange(pairs);
            return this;
        }

        /// <inheritdoc/>
        public override JsonWriter WriteJson(JsonWriter writer)
        {
            writer.WritePropertyName("inputs");
            writer.WriteStartArray();

            var s = new JsonSerializer();

            foreach (var keyData in bucketKeyKeyData)
            {
                writer.WriteStartArray();
                writer.WriteValue(keyData.ObjectId.Bucket);
                writer.WriteValue(keyData.ObjectId.Key);
                s.Serialize(writer, keyData.KeyData);
                if (keyData.ObjectId.BucketType != null)
                {
                    writer.WriteValue(keyData.ObjectId.BucketType);
                }

                writer.WriteEndArray();
            }

            writer.WriteEndArray();

            return writer;
        }

        private void AddRange(IEnumerable<Tuple<RiakObjectId, object>> pairs)
        {
            bucketKeyKeyData.AddRange(pairs.Select(p => new RiakBucketKeyKeyDataInputItem(p)));
        }

        private class RiakBucketKeyKeyDataInputItem
        {
            private readonly RiakObjectId objectId;
            private readonly object keyData;

            public RiakBucketKeyKeyDataInputItem(RiakObjectId objectId, object keyData)
            {
                this.objectId = objectId;
                this.keyData = keyData;
            }

            // Helper constructor
            public RiakBucketKeyKeyDataInputItem(Tuple<RiakObjectId, object> data)
                : this(data.Item1, data.Item2)
            {
            }

            // Helper constructor for old-style RiakBucketKeyKeyDataInput builder methods.
            // Will be removed in future.
            public RiakBucketKeyKeyDataInputItem(Tuple<string, string, object> data)
                : this(new RiakObjectId(data.Item1, data.Item2), data.Item3)
            {
            }

            public RiakObjectId ObjectId
            {
                get { return objectId; }
            }

            public object KeyData
            {
                get { return keyData; }
            }
        }
    }
}
