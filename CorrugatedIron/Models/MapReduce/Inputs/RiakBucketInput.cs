// Copyright (c) 2011 - OJ Reeves & Jeremiah Peschka
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

using CorrugatedIron.Models.MapReduce.KeyFilters;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace CorrugatedIron.Models.MapReduce.Inputs
{
    public class RiakBucketInput : RiakPhaseInput
    {
        private readonly string _bucket;

        public RiakBucketInput(string bucket)
        {
            _bucket = bucket;
            Filters = new List<IRiakKeyFilterToken>();
        }

        public override JsonWriter WriteJson(JsonWriter writer)
        {
            if(Filters.Count > 0)
            {
                writer.WritePropertyName("inputs");
                writer.WriteStartObject();

                writer.WritePropertyName("bucket");
                writer.WriteValue(_bucket);

                writer.WritePropertyName("key_filters");
                writer.WriteStartArray();

                Filters.ForEach(f => writer.WriteRawValue(f.ToJsonString()));

                writer.WriteEndArray();
                writer.WriteEndObject();
            }
            else
            {
                writer.WritePropertyName("inputs");
                writer.WriteValue(_bucket);
            }

            return writer;
        }

        public static implicit operator RiakBucketInput(string bucket)
        {
            return new RiakBucketInput(bucket);
        }
    }
}
