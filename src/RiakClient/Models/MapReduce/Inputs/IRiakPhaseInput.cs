// <copyright file="IRiakPhaseInput.cs" company="Basho Technologies, Inc.">
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

using Newtonsoft.Json;
using System.Collections.Generic;
using RiakClient.Models.MapReduce.KeyFilters;

namespace RiakClient.Models.MapReduce.Inputs
{
    public interface IRiakPhaseInput
    {
        JsonWriter WriteJson(JsonWriter writer);
    }

    public abstract class RiakPhaseInput : IRiakPhaseInput
    {
        public List<IRiakKeyFilterToken> Filters { get; set; }
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
