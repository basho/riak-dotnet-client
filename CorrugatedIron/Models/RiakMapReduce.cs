// Copyright (c) 2010 - OJ Reeves & Jeremiah Peschka
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

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using CorrugatedIron.Extensions;
using CorrugatedIron.KeyFilters;
using CorrugatedIron.Messages;
using CorrugatedIron.Util;
using Newtonsoft.Json;

namespace CorrugatedIron.Models
{
    public struct BucketKeyData
    {
        public string Bucket;
        public string Key;
        public string KeyData;
    }

    public class RiakMapReduce
    {
        public RiakMapReduce()
        {
            MapReducePhases = new Dictionary<string, RiakMapReducePhase>();
        }

        // TODO create and implement Bucket class
        public RiakMapReduce(string request, string contentType = Constants.ContentTypes.ApplicationJson) : this()
        {
            Request = request;
            ContentType = contentType;
        }

        public RiakMapReduce(string bucket, string request = "",
                             string contentType = Constants.ContentTypes.ApplicationJson) : this()
        {
            Bucket = bucket;
            ContentType = contentType;
            Request = request;
        }

        public List<IRiakKeyFilterToken> Filters { get; set; }
        public Dictionary<string, RiakMapReducePhase> MapReducePhases { get; set; }

        public string Bucket { get; set; }
        public string Request { get; set; }
        public string Inputs { get; set; }
        // TODO push this out to the client for a given request.
        // This could be exposed via the client interface as part of a configuration object
        public string ContentType { get; set; }

        /// <summary>
        /// Set the inputs property of the MapReduce job to a single bucket
        /// </summary>
        /// <param name="bucket"></param>
        /// <returns></returns>
        public RiakMapReduce SetInputs(string bucket)
        {
            Inputs = bucket;

            return this;
        }

        /// <summary>
        /// Set the inputs property of the MapReduce job to a list of Bucket, Key pairs
        /// </summary>
        /// <param name="bucketKeyPairs"></param>
        /// <returns></returns>
        public RiakMapReduce SetInputs(IDictionary<string, string> bucketKeyPairs)
        {
            var sb = new StringBuilder();
            var sw = new StringWriter(sb);

            using (JsonWriter jw = new JsonTextWriter(sw))
            {
                jw.WriteStartArray();

                bucketKeyPairs.ForEach(kv => WriteBucketKeyPair(kv, jw));

                jw.WriteEndArray();
            }

            Inputs = sb.ToString();

            return this;
        }

        public RiakMapReduce SetInputs(List<BucketKeyData> bucketKeyDatas)
        {
            var sb = new StringBuilder();
            var sw = new StringWriter(sb);

            using (JsonWriter jw = new JsonTextWriter(sw))
            {
                jw.WriteStartArray();

                bucketKeyDatas.ForEach(bkd => WriteBucketKeyData(bkd, jw));

                jw.WriteEndArray();
            }

            Inputs = sb.ToString();

            return this;
        }

        // TODO implement inputs as a RiakSearch query

        // TODO implement tests for transforming MR jobs into Json strings
        public RiakMapReduce Filter(IRiakKeyFilterToken filter)
        {
            Filters.Add(filter);

            return this;
        }

        // TODO add support for passing arguments
        public RiakMapReduce Map(string module, string function, object[] args = null)
        {
            throw new NotImplementedException();
        }

        public RiakMapReduce Map(bool keep, string language, string source = "", string name = "", string arg = "")
        {
            AddMapReducePhase(keep, language, source, name, Constants.MapReducePhaseType.Map, arg);

            return this;
        }

        public RiakMapReduce Reduce(bool keep, string language, string source = "", string name = "")
        {
            AddMapReducePhase(keep, language, source, name, Constants.MapReducePhaseType.Reduce, "");

            return this;
        }

        public RiakMapReduce Link()
        {
            throw new NotImplementedException();
        }

        public RpbMapRedReq ToMessage()
        {
            if (string.IsNullOrEmpty(Request))
            {
                var sb = new StringBuilder();
                MapReducePhases.ForEach(mr => sb.Append(mr.Value.ToString()));
                Request = sb.ToString();
            }

            var message = new RpbMapRedReq
                              {
                                  Request = Request.ToRiakString(),
                                  ContentType = ContentType.ToRiakString()
                              };

            return message;
        }

        private void AddMapReducePhase(bool keep, string language, string source, string name, string mapReducePhaseType,
                                       string arg)
        {
            var phase = new RiakMapReducePhase()
                            {
                                MapReducePhaseType = mapReducePhaseType,
                                MapReduceLanguage =  language,
                                Keep = keep,
                                Source = source,
                                Name = name,
                                Argument = arg
                            };
            MapReducePhases.Add(phase.MapReducePhaseType, phase);
        }

        private static void WriteBucketKeyPair(KeyValuePair<string, string> bucketKeyPair, JsonWriter writer)
        {
            writer.WriteStartArray();
            writer.WriteValue(bucketKeyPair.Key);
            writer.WriteValue(bucketKeyPair.Value);
            writer.WriteEndArray();
        }

        private static void WriteBucketKeyData(BucketKeyData bucketKeyData, JsonWriter writer)
        {
            writer.WriteStartArray();
            writer.WriteValue(bucketKeyData.Bucket);
            writer.WriteValue(bucketKeyData.Key);
            writer.WriteValue(bucketKeyData.KeyData);
            writer.WriteEnd();
        }
    }
}