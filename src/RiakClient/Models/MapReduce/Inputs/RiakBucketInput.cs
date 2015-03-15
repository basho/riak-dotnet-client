// <copyright file="RiakBucketInput.cs" company="Basho Technologies, Inc.">
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
    using System.Collections.Generic;
    using KeyFilters;
    using Newtonsoft.Json;

    /// <summary>
    /// Represents a bucket based mapreduce input.
    /// </summary>
    public class RiakBucketInput : RiakPhaseInput
    {
        private readonly string bucket;
        private readonly string type;

        /// <summary>
        /// Initializes a new instance of the <see cref="RiakBucketInput"/> class.
        /// </summary>
        /// <param name="bucket">The bucket to use as input for the mapreduce job.</param>
        public RiakBucketInput(string bucket)
            : this(RiakConstants.DefaultBucketType, bucket)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RiakBucketInput"/> class.
        /// </summary>
        /// <param name="bucketType">The bucket type containing the <paramref name="bucket"/>.</param>
        /// <param name="bucket">The bucket to use as input for the mapreduce job.</param>
        public RiakBucketInput(string bucketType, string bucket)
        {
            this.type = bucketType;
            this.bucket = bucket;
            Filters = new List<IRiakKeyFilterToken>();
        }

        /// <inheritdoc/>
        public override JsonWriter WriteJson(JsonWriter writer)
        {
            if (Filters.Count > 0)
            {
                writer.WritePropertyName("inputs");
                writer.WriteStartObject();

                WriteBucketKeyBucketJson(writer, type, bucket);

                writer.WritePropertyName("key_filters");
                writer.WriteStartArray();

                Filters.ForEach(f => writer.WriteRawValue(f.ToJsonString()));

                writer.WriteEndArray();
                writer.WriteEndObject();
            }
            else
            {
                writer.WritePropertyName("inputs");
                writer.WriteValue(bucket);
            }

            return writer;
        }
    }
}
