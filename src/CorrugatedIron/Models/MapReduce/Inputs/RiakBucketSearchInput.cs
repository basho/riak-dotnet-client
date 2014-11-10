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

using CorrugatedIron.Models.Search;
using Newtonsoft.Json;

namespace CorrugatedIron.Models.MapReduce.Inputs
{
    public class RiakBucketSearchInput : RiakPhaseInput
    {
        private readonly string _bucket;
        private readonly string _query;
        private string _filter;

        public RiakBucketSearchInput(RiakFluentSearch query)
            : this(query.Bucket, query.ToString())
        {
        }

        public RiakBucketSearchInput(string bucket, string query)
        {
            _bucket = bucket;
            _query = query;
        }

        public RiakBucketSearchInput Filter(RiakFluentSearch filter)
        {
            return Filter(filter.ToString());
        }

        public RiakBucketSearchInput Filter(string filter)
        {
            _filter = filter;
            return this;
        }

        public override JsonWriter WriteJson(JsonWriter writer)
        {
            writer.WritePropertyName("inputs");
            writer.WriteStartObject();

            writer.WritePropertyName("bucket");
            writer.WriteValue(_bucket);

            writer.WritePropertyName("query");
            writer.WriteValue(_query);

            if (!string.IsNullOrEmpty(_filter))
            {
                writer.WritePropertyName("filter");
                writer.WriteValue(_filter);
            }

            writer.WriteEndObject();

            return writer;
        }
    }
}
