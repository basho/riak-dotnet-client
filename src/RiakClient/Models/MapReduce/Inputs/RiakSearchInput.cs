// <copyright file="RiakSearchInput.cs" company="Basho Technologies, Inc.">
// Copyright (c) 2011 - OJ Reeves & Jeremiah Peschka
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
// </copyright>

using Newtonsoft.Json;
using RiakClient.Models.Search;

namespace RiakClient.Models.MapReduce.Inputs
{
    public class RiakSearchInput : RiakPhaseInput
    {
        private readonly string _index;
        private readonly string _query;

        public RiakSearchInput(RiakFluentSearch query)
            : this(query.Index, query.ToString())
        {
        }

        public RiakSearchInput(string index, string query)
        {
            _index = index;
            _query = query;
        }

        public override JsonWriter WriteJson(JsonWriter writer)
        {
            writer.WritePropertyName("inputs");
            writer.WriteStartObject();

            writer.WritePropertyName("index");
            writer.WriteValue(_index);

            writer.WritePropertyName("query");
            writer.WriteValue(_query);

            writer.WriteEndObject();

            return writer;
        }
    }
}
