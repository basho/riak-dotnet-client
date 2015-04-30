// <copyright file="RiakBucketKeyInput.cs" company="Basho Technologies, Inc.">
// Copyright 2011 - OJ Reeves & Jeremiah Peschka
// Copyright 2014 - Basho Technologies, Inc.
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
    /// Represents a Bucket/Key collection based mapreduce input.
    /// </summary>
    public class RiakBucketKeyInput : RiakPhaseInput
    {
        private readonly List<RiakObjectId> riakObjectIdList = new List<RiakObjectId>();

        /// <summary>
        /// Initializes a new instance of the <see cref="RiakBucketKeyInput"/> class, 
        /// and adds the attached <paramref name="riakObjectIds"/> to the collection of inputs.
        /// </summary>
        /// <param name="riakObjectIds">The object ids to add to the input collection.</param>
        /// <returns>The newly created <see cref="RiakBucketKeyInput"/>.</returns>
        public static RiakBucketKeyInput FromRiakObjectIds(IEnumerable<RiakObjectId> riakObjectIds)
        {
            var bucketKeyInput = new RiakBucketKeyInput();
            bucketKeyInput.Add(riakObjectIds);
            return bucketKeyInput;
        }

        /// <summary>
        /// Adds a (bucket, key) pair to the mapreduce inputs collection.
        /// </summary>
        /// <param name="bucket">The bucket of the object address to add to the input collection.</param>
        /// <param name="key">The key of the object address to add to the input collection.</param>
        /// <returns>A reference to this updated instance, for fluent chaining.</returns>
        [Obsolete("Use the Add() that accepts a RiakObjectId instead. This will be removed in the next version.")]
        public RiakBucketKeyInput Add(string bucket, string key)
        {
            riakObjectIdList.Add(new RiakObjectId(bucket, key));
            return this;
        }

        /// <summary>
        /// Adds a <see cref="RiakObjectId"/> to the mapreduce inputs collection
        /// </summary>
        /// <param name="objectId">The <see cref="RiakObjectId"/> to add to the input collection.</param>
        /// <returns>A reference to this updated instance, for fluent chaining.</returns>
        public RiakBucketKeyInput Add(RiakObjectId objectId)
        {
            riakObjectIdList.Add(objectId);
            return this;
        }

        /// <summary>
        /// Adds a collection of <see cref="RiakObjectId"/> to the mapreduce inputs collection.
        /// </summary>
        /// <param name="objectIds">The <see cref="RiakObjectId"/> params array to add to the input collection.</param>
        /// <returns>A reference to this updated instance, for fluent chaining.</returns>
        public RiakBucketKeyInput Add(params RiakObjectId[] objectIds)
        {
            riakObjectIdList.AddRange(objectIds);
            return this;
        }

        /// <summary>
        /// Adds a collection of <see cref="RiakObjectId"/> to the mapreduce inputs collection.
        /// </summary>
        /// <param name="objectIds">The <see cref="RiakObjectId"/> collection to add to the input collection.</param>
        /// <returns>A reference to this updated instance, for fluent chaining.</returns>
        public RiakBucketKeyInput Add(IEnumerable<RiakObjectId> objectIds)
        {
            riakObjectIdList.AddRange(objectIds);
            return this;
        }

        /// <summary>
        /// Adds a collection of (bucket, key) pairs to the mapreduce inputs collection.
        /// </summary>
        /// <param name="pairs">The (bucket, key) pairs to add to the input collection.</param>
        /// <returns>A reference to this updated instance, for fluent chaining.</returns>
        [Obsolete("Use the Add() that accepts RiakObjectId[] instead. This will be removed in the next version.")]
        public RiakBucketKeyInput Add(params Tuple<string, string>[] pairs)
        {
            riakObjectIdList.AddRange(pairs.Select(p => new RiakObjectId(p.Item1, p.Item2)));
            return this;
        }

        /// <summary>
        /// Adds a collection of (bucket, key) pairs to the mapreduce inputs collection.
        /// </summary>
        /// <param name="pairs">The (bucket, key) pairs to add to the input collection.</param>
        /// <returns>A reference to this updated instance, for fluent chaining.</returns>
        [Obsolete("Use the Add() that accepts an IEnumerable<RiakObjectId> instead. This will be removed in the next version.")]
        public RiakBucketKeyInput Add(IEnumerable<Tuple<string, string>> pairs)
        {
            riakObjectIdList.AddRange(pairs.Select(p => new RiakObjectId(p.Item1, p.Item2)));
            return this;
        }

        /// <inheritdoc/>
        public override JsonWriter WriteJson(JsonWriter writer)
        {
            writer.WritePropertyName("inputs");
            writer.WriteStartArray();

            foreach (var id in riakObjectIdList)
            {
                WriteRiakObjectIdToWriter(writer, id);
            }

            writer.WriteEndArray();

            return writer;
        }

        private static void WriteRiakObjectIdToWriter(JsonWriter writer, RiakObjectId id)
        {
            writer.WriteStartArray();
            writer.WriteValue(id.Bucket);
            writer.WriteValue(id.Key);
            writer.WriteValue(id.BucketType ?? string.Empty);
            writer.WriteEndArray();
        }
    }
}
