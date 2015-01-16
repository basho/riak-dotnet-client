// Copyright (c) 2010 - OJ Reeves & Jeremiah Peschka
// 
// This file is provided to you under the Apache License,
// Version 2.0 (the "License"); you may not use this file
// except in compliance with the License.  You may obtain
// a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing,
// software distributed under the License is distributed on an
// "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
// KIND, either express or implied.  See the License for the
// specific language governing permissions and limitations
// under the License.

using System;
using CorrugatedIron.Extensions;
using Newtonsoft.Json;

namespace CorrugatedIron.Models.MapReduce.Inputs
{
    public class RiakBinIndexEqualityInput : RiakIndexInput
    {
        [Obsolete("Use IndexId.BucketName instead. This will be revoved in the next version.")]
        public string Bucket 
        { 
            get 
            {
                return IndexId != null ? IndexId.BucketName : null;
            }
        }

        [Obsolete("Use IndexId.IndexName instead. This will be revoved in the next version.")]
        public string Index
        {
            get
            {
                return IndexId != null ? IndexId.IndexName : null;
            }
        }

        public string Key { get; set; }

        [Obsolete("Use the constructor that accepts a RiakIndexId instead. This will be revoved in the next version.")]
        public RiakBinIndexEqualityInput(string bucket, string index, string key)
            : this(new RiakIndexId(bucket, index), key)
        {
        }

        public RiakBinIndexEqualityInput(RiakIndexId indexId, string key)
        {
            // TODO: Check on where we usually add the index suffixes. 
            IndexId = new RiakIndexId(indexId.BucketType, indexId.BucketName, indexId.IndexName.ToBinaryKey());
            Key = key;
        }

        public override JsonWriter WriteJson(JsonWriter writer)
        {
            WriteIndexHeaderJson(writer);

            writer.WritePropertyName("key");
            writer.WriteValue(Key);
            writer.WriteEndObject();

            return writer;
        }
    }
}
