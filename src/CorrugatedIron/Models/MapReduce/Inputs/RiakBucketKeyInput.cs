// Copyright (c) 2013 - OJ Reeves & Jeremiah Peschka
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

namespace CorrugatedIron.Models.MapReduce.Inputs
{
    public class RiakBucketKeyInput : RiakPhaseInput
    {
        private List<RiakObjectId> RiakObjectIdList { get; set; }
        
        public RiakBucketKeyInput()
        {
            RiakObjectIdList = new List<RiakObjectId>();
        }

        public RiakBucketKeyInput Add(string bucket, string key)
        {
            RiakObjectIdList.Add(new RiakObjectId(bucket, key));
            return this;
        }

        public RiakBucketKeyInput Add(RiakObjectId objectId)
        {
            RiakObjectIdList.Add(objectId);
            return this;
        }

        public RiakBucketKeyInput Add(params RiakObjectId[] objectIds)
        {
            RiakObjectIdList.AddRange(objectIds);
            return this;
        }

        public RiakBucketKeyInput Add(IEnumerable<RiakObjectId> objectIds)
        {
            RiakObjectIdList.AddRange(objectIds);
            return this;
        }

        public RiakBucketKeyInput Add(params Tuple<string, string>[] pairs)
        {
            RiakObjectIdList.AddRange(pairs.Select(p => new RiakObjectId(p.Item1, p.Item2)));
            return this;
        }

        public RiakBucketKeyInput Add(IEnumerable<Tuple<string, string>> pairs)
        {
            RiakObjectIdList.AddRange(pairs.Select(p => new RiakObjectId(p.Item1, p.Item2)));
            return this;
        }

        public override JsonWriter WriteJson(JsonWriter writer)
        {
            writer.WritePropertyName("inputs");
            writer.WriteStartArray();

            foreach (var id in RiakObjectIdList)
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

        public static RiakBucketKeyInput FromRiakObjectIds(IEnumerable<RiakObjectId> riakObjectIds)
        {
            var rbki = new RiakBucketKeyInput();
            rbki.Add(riakObjectIds);
            return rbki;
        }
    }
}
