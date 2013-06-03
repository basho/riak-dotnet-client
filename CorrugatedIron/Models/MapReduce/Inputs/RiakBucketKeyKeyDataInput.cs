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

using System.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace CorrugatedIron.Models.MapReduce.Inputs
{
    public class RiakBucketKeyKeyDataInput : RiakPhaseInput
    {
        private List<Tuple<string, string, object>> BucketKeyKeyData { get; set; }

        public RiakBucketKeyKeyDataInput()
        {
            BucketKeyKeyData = new List<Tuple<string, string, object>>();
        }

        public RiakBucketKeyKeyDataInput Add(string bucket, string key, object keyData)
        {
            BucketKeyKeyData.Add(new Tuple<string, string, object>(bucket, key, keyData));
            return this;
        }

        public RiakBucketKeyKeyDataInput Add(params Tuple<string, string, object>[] pairs)
        {
            BucketKeyKeyData.AddRange(pairs);
            return this;
        }

        public RiakBucketKeyKeyDataInput Add(IEnumerable<Tuple<string, string, object>> pairs)
        {
            BucketKeyKeyData.AddRange(pairs);
            return this;
        }

        public override JsonWriter WriteJson(JsonWriter writer)
        {
            writer.WritePropertyName("inputs");
            writer.WriteStartArray();

            var s = new JsonSerializer();

            BucketKeyKeyData.ForEach(bkkd =>
            {
                writer.WriteStartArray();
                writer.WriteValue(bkkd.Item1);
                writer.WriteValue(bkkd.Item2);
                s.Serialize(writer, bkkd.Item3);
                writer.WriteEndArray();
            });

            writer.WriteEndArray();

            return writer;
        }
    }
}
