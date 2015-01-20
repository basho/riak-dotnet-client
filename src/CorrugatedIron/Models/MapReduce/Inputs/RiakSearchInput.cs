﻿// Copyright (c) 2011 - 2014 OJ Reeves & Jeremiah Peschka
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

using CorrugatedIron.Models.Search;
using Newtonsoft.Json;

namespace CorrugatedIron.Models.MapReduce.Inputs
{
    public class RiakSearchInput : RiakPhaseInput
    {
        private readonly string _index;
        private readonly string _query;
        private string _filter;

        public RiakSearchInput(RiakFluentSearch query)
            : this(query.Index, query.ToString())
        {
        }

        public RiakSearchInput(string index, string query)
        {
            _index = index;
            _query = query;
        }

        public RiakSearchInput Filter(RiakFluentSearch filter)
        {
            return Filter(filter.ToString());
        }

        public RiakSearchInput Filter(string filter)
        {
            _filter = filter;
            return this;
        }

        public override JsonWriter WriteJson(JsonWriter writer)
        {
            writer.WritePropertyName("inputs");
            writer.WriteStartObject();

            writer.WritePropertyName("module");
            writer.WriteValue("yokozuna");

            writer.WritePropertyName("function");
            writer.WriteValue("mapred_search");

            writer.WritePropertyName("arg");
            writer.WriteStartArray();

            writer.WriteValue(_index);
            writer.WriteValue(_query);

            if (!string.IsNullOrEmpty(_filter))
            {
                writer.WritePropertyName("filter");
                writer.WriteValue(_filter);
            }

            writer.WriteEndArray();

            writer.WriteEndObject();

            return writer;
        }
    }
}