// Copyright (c) 2011 - 2014 OJ Reeves & Jeremiah Peschka
// Copyright (c) 2015 - Basho Technologies, Inc.
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
using Newtonsoft.Json;

namespace RiakClient.Models.MapReduce.Inputs
{
    public abstract class RiakIndexInput : RiakPhaseInput
    {
        private readonly RiakIndexId _indexId;

        protected RiakIndexInput(RiakIndexId indexId)
        {
            _indexId = indexId;
        }

        public RiakIndexId IndexId
        {
            get { return _indexId; }
        }

        [Obsolete("Use IndexId.BucketName instead. This will be removed in the next version.")]
        public string Bucket
        {
            get { return IndexId != null ? IndexId.BucketName : null; }
        }

        [Obsolete("Use IndexId.IndexName instead. This will be removed in the next version.")]
        public string Index
        {
            get { return IndexId != null ? IndexId.IndexName : null; }
        }

        protected void WriteIndexHeaderJson(JsonWriter writer)
        {
            writer.WritePropertyName("inputs");

            writer.WriteStartObject();

            WriteBucketKeyBucketJson(writer, IndexId.BucketType, IndexId.BucketName);

            writer.WritePropertyName("index");
            writer.WriteValue(IndexId.IndexName);
        }
    }
}
