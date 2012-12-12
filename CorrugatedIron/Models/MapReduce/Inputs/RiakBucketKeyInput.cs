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

using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace CorrugatedIron.Models.MapReduce.Inputs
{
    public class RiakBucketKeyInput : RiakPhaseInput
    {
        private List<Tuple<string, string>> BucketKeyList { get; set; }

        public RiakBucketKeyInput()
        {
            BucketKeyList = new List<Tuple<string, string>>();
        }

        public void AddBucketKey(string bucket, string key)
        {
            BucketKeyList.Add(new Tuple<string, string>(bucket, key));
        }

        public override JsonWriter WriteJson(JsonWriter writer)
        {
            writer.WritePropertyName("inputs");
            writer.WriteStartArray();

            BucketKeyList.ForEach(bk =>
            {
                writer.WriteStartArray();
                writer.WriteValue(bk.Item1);
                writer.WriteValue(bk.Item2);
                writer.WriteEndArray();
            });

            writer.WriteEndArray();

            return writer;
        }
        
        public static RiakBucketKeyInput FromRiakObjectIds(IEnumerable<RiakObjectId> riakObjectIds)
        {
            var rbki = new RiakBucketKeyInput();
            
            foreach(var objectid in riakObjectIds)
            {
                rbki.AddBucketKey(objectid.Bucket, objectid.Key);
            }
            
            return rbki;
        }
    }
}
