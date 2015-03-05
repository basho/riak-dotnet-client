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

namespace RiakClient.Models.MapReduce.Inputs
{
    using Models.Search;
    using Newtonsoft.Json;

    /// <summary>
    /// Represents a search based mapreduce input.
    /// </summary>
    public class RiakSearchInput : RiakPhaseInput
    {
        private readonly string index;
        private readonly string query;

        /// <summary>
        /// Initializes a new instance of the <see cref="RiakSearchInput"/> class.
        /// </summary>
        /// <param name="query">The <see cref="RiakFluentSearch"/> to run, whose results will be used as inputs for the mapreduce job. </param>
        public RiakSearchInput(RiakFluentSearch query)
            : this(query.Index, query.ToString())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RiakSearchInput"/> class.
        /// </summary>
        /// <param name="index">The index to run the <paramref name="query"/> against.</param>
        /// <param name="query">The query to run, whose results will be used as inputs for the mapreduce job.</param>
        public RiakSearchInput(string index, string query)
        {
            this.index = index;
            this.query = query;
        }

        /// <inheritdoc/>
        public override JsonWriter WriteJson(JsonWriter writer)
        {
            writer.WritePropertyName("inputs");
            writer.WriteStartObject();

            writer.WritePropertyName("index");
            writer.WriteValue(index);

            writer.WritePropertyName("query");
            writer.WriteValue(query);

            writer.WriteEndObject();

            return writer;
        }
    }
}
