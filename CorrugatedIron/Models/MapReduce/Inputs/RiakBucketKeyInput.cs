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
        private List<Tuple<string, string>> BucketKeyList { get; set; }

        public RiakBucketKeyInput()
        {
            BucketKeyList = new List<Tuple<string, string>>();
        }

        public RiakBucketKeyInput Add(string bucket, string key)
        {
            BucketKeyList.Add(Tuple.Create(bucket, key));
            return this;
        }

        public RiakBucketKeyInput Add(RiakObjectId objectId)
        {
            BucketKeyList.Add(Tuple.Create(objectId.Bucket, objectId.Key));
            return this;
        }

        public RiakBucketKeyInput Add(params RiakObjectId[] objectIds)
        {
            BucketKeyList.AddRange(objectIds.Select(o => Tuple.Create(o.Bucket, o.Key)));
            return this;
        }

        public RiakBucketKeyInput Add(IEnumerable<RiakObjectId> objectIds)
        {
            BucketKeyList.AddRange(objectIds.Select(o => Tuple.Create(o.Bucket, o.Key)));
            return this;
        }

        public RiakBucketKeyInput Add(params Tuple<string, string>[] pairs)
        {
            BucketKeyList.AddRange(pairs);
            return this;
        }

        public RiakBucketKeyInput Add(IEnumerable<Tuple<string, string>> pairs)
        {
            BucketKeyList.AddRange(pairs);
            return this;
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
            rbki.Add(riakObjectIds);
            return rbki;
        }
    }
}
