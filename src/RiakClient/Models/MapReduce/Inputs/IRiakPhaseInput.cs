// <copyright file="IRiakPhaseInput.cs" company="Basho Technologies, Inc.">
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
    using System.Collections.Generic;
    using Models.MapReduce.KeyFilters;
    using Newtonsoft.Json;

    /// <summary>
    /// Interface for mapreduce phase inputs.
    /// </summary>
    public interface IRiakPhaseInput
    {
        /// <summary>
        /// Serialize the phase input to JSON and write it using the <paramref name="writer"/>.
        /// </summary>
        /// <param name="writer">The writer to use.</param>
        /// <returns>The original JsonWriter, not modified but useful for call chaining.</returns>
        JsonWriter WriteJson(JsonWriter writer);
    }
    
    /// <summary>
    /// Abstract implementation of <see cref="IRiakPhaseInput"/>. 
    /// </summary>
    public abstract class RiakPhaseInput : IRiakPhaseInput
    {
        /// <summary>
        /// A list of key filters represented as <see cref="IRiakKeyFilterToken"/>s.
        /// </summary>
        public List<IRiakKeyFilterToken> Filters { get; set; }

        /// <inheritdoc/>
        public abstract JsonWriter WriteJson(JsonWriter writer);

        protected void WriteBucketKeyBucketJson(JsonWriter writer, string bucketType, string bucketName)
        {
            writer.WritePropertyName("bucket");

            if (string.IsNullOrEmpty(bucketType))
            {
                writer.WriteValue(bucketName);
            }
            else
            {
                writer.WriteStartArray();
                writer.WriteValue(bucketType);
                writer.WriteValue(bucketName);
                writer.WriteEndArray();
            }
        }
    }
}
