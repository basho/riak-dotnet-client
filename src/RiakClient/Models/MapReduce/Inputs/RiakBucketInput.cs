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
    using Models.MapReduce.KeyFilters;
    using Newtonsoft.Json;

    public class RiakBucketInput : RiakPhaseInput
    {
        private readonly string bucket;
        private readonly string type;

        public RiakBucketInput(string bucket, string type = null)
        {
            this.type = type;
            this.bucket = bucket;
            Filters = new List<IRiakKeyFilterToken>();
        }

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
