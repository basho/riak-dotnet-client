// <copyright file="RiakMapReduceQuery.cs" company="Basho Technologies, Inc.">
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

namespace RiakClient.Models.MapReduce
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Text;
    using Extensions;
    using Fluent;
    using Inputs;
    using KeyFilters;
    using Languages;
    using Messages;
    using Newtonsoft.Json;
    using Phases;

    /// <summary>
    /// A fluent builder class for Riak MapReduce queries.
    /// </summary>
    public class RiakMapReduceQuery
    {
        private readonly List<RiakPhase> phases = new List<RiakPhase>();
        private readonly string contentType = RiakConstants.ContentTypes.ApplicationJson;
        private readonly int? timeout = null;

        private string query;
        private RiakPhaseInput inputs;

        /// <summary>
        /// Initializes a new instance of the <see cref="RiakMapReduceQuery"/> class.
        /// </summary>
        /// <remarks>
        /// This overload defaults the content-type to "application/json", with the default timeout.
        /// </remarks>
#pragma warning disable 618
        public RiakMapReduceQuery() : this(RiakConstants.ContentTypes.ApplicationJson, null)
#pragma warning restore 618
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RiakMapReduceQuery"/> class.
        /// </summary>
        /// <param name="timeout">The timeout period for the query, in milliseconds.</param>
        /// <remarks>
        /// This overload defaults the content-type to "application/json".
        /// </remarks>
        public RiakMapReduceQuery(int? timeout = null)
        {
            this.contentType = RiakConstants.ContentTypes.ApplicationJson;
            this.timeout = timeout;
        }

        // TODO: Remove this overload - contentType is always json.

        /// <summary>
        /// Initializes a new instance of the <see cref="RiakMapReduceQuery"/> class.
        /// </summary>
        /// <param name="contentType">The query's return content-type.</param>
        /// <param name="timeout">The timeout period for the query, in milliseconds.</param>
        /// <exception cref="ArgumentException">The value of 'contentType' cannot be null, empty, or whitespace.</exception>
        [Obsolete("This overload will be removed in the next version, please use the RiakMapReduceQuery() " +
                  "or RiakMapReduceQuery(int? timeout) overloads instead.")]
        public RiakMapReduceQuery(string contentType, int? timeout = null)
        {
            if (string.IsNullOrWhiteSpace(contentType))
            {
                throw new ArgumentException("The value of 'contentType' cannot be null, empty, or whitespace.");
            }

            this.contentType = contentType;
            this.timeout = timeout;
        }

        /// <summary>
        /// The content-type of the mapreduce query.
        /// </summary>
        public string ContentType
        {
            get { return contentType; }
        }

        /// <summary>
        /// The optional timeout for the mapreduce query.
        /// </summary>
        public int? Timeout
        {
            get { return timeout; }
        }

        /// <summary>
        /// Add a bucket to the list of inputs.
        /// </summary>
        /// <param name="bucket">The Bucket to add to the list of inputs.</param>
        /// <returns>A reference to this updated instance, for fluent chaining.</returns>
        public RiakMapReduceQuery Inputs(string bucket)
        {
            inputs = new RiakBucketInput(bucket);
            return this;
        }

        /// <summary>
        /// Add a bucket to the list of inputs.
        /// </summary>
        /// <param name="bucketType">The bucket type containing the bucket to use as input.</param>
        /// <param name="bucket">The BucketType/Bucket to add to the list of inputs.</param>
        /// <returns>A reference to this updated instance, for fluent chaining.</returns>
        public RiakMapReduceQuery Inputs(string bucketType, string bucket)
        {
            inputs = new RiakBucketInput(bucketType, bucket);
            return this;
        }

        /// <summary>
        /// Add a bucket-based legacy search input to the list of inputs.
        /// </summary>
        /// <param name="riakBucketSearchInput">The <see cref="RiakBucketSearchInput"/> to add to the list of inputs.</param>
        /// <returns>A reference to this updated instance, for fluent chaining.</returns>
        [Obsolete("Using Legacy Search as input for MapReduce is deprecated. Please move to Riak 2.0 Search, and use the RiakSearchInput class instead.")]
        public RiakMapReduceQuery Inputs(RiakBucketSearchInput riakBucketSearchInput)
        {
            inputs = riakBucketSearchInput;
            return this;
        }

        /// <summary>
        /// Add an index-based search input to the list of inputs.
        /// </summary>
        /// <param name="riakSearchInput">The <see cref="RiakSearchInput"/> to add to the list of inputs.</param>
        /// <returns>A reference to this updated instance, for fluent chaining.</returns>
        /// <remarks>Riak 2.0+ only.</remarks>
        public RiakMapReduceQuery Inputs(RiakSearchInput riakSearchInput)
        {
            inputs = riakSearchInput;
            return this;
        }

        /// <summary>
        /// Add a collection of <see cref="RiakObjectId"/>'s to the list of inputs.
        /// </summary>
        /// <param name="riakBucketKeyInputs">The <see cref="RiakBucketKeyInput"/> to add to the list of inputs.</param>
        /// <returns>A reference to this updated instance, for fluent chaining.</returns>
        public RiakMapReduceQuery Inputs(RiakBucketKeyInput riakBucketKeyInputs)
        {
            inputs = riakBucketKeyInputs;
            return this;
        }

        /// <summary>
        /// Add a collection of <see cref="RiakObjectId"/> / KeyData sets to the list of inputs.
        /// </summary>
        /// <param name="riakBucketKeyKeyDataInputs">The <see cref="RiakBucketKeyKeyDataInput"/> to add to the list of inputs.</param>
        /// <returns>A reference to this updated instance, for fluent chaining.</returns>
        public RiakMapReduceQuery Inputs(RiakBucketKeyKeyDataInput riakBucketKeyKeyDataInputs)
        {
            inputs = riakBucketKeyKeyDataInputs;
            return this;
        }

        /// <summary>
        /// Add an Erlang Module/Function/Arg combo to the list of inputs.
        /// </summary>
        /// <param name="riakModFunArgsInput">The <see cref="RiakModuleFunctionArgInput"/> to add to the list of inputs.</param>
        /// <returns>A reference to this updated instance, for fluent chaining.</returns>
        public RiakMapReduceQuery Inputs(RiakModuleFunctionArgInput riakModFunArgsInput)
        {
            inputs = riakModFunArgsInput;
            return this;
        }

        /// <summary>
        /// Add a secondary index query to the list of inputs.
        /// </summary>
        /// <param name="riakIndexPhaseInput">The <see cref="RiakIndexInput"/> to add to the list of inputs.</param>
        /// <returns>A reference to this updated instance, for fluent chaining.</returns>
        public RiakMapReduceQuery Inputs(RiakIndexInput riakIndexPhaseInput)
        {
            inputs = riakIndexPhaseInput;
            return this;
        }

        /// <summary>
        /// Add an Erlang map phase to the list of phases.
        /// </summary>
        /// <param name="setup">
        /// An <see cref="Action{T}"/> that accepts a <see cref="RiakFluentActionPhaseJavascript"/>,
        /// and configures the map phase with it.
        /// </param>        
        /// <returns>A reference to this updated instance, for fluent chaining.</returns>
        /// <remarks>
        /// Configure the phase with a lambda similar to:
        /// <code>.MapErlang(m => m.ModFun("riak_kv_mapreduce", "map_object_value"))</code>
        /// The above code will run the "riak_kv_mapreduce:map_object_value" Erlang function on each input in the phase.. 
        /// </remarks>
        public RiakMapReduceQuery MapErlang(Action<RiakFluentActionPhaseErlang> setup)
        {
            var phase = new RiakMapPhase<RiakPhaseLanguageErlang>();
            var fluent = new RiakFluentActionPhaseErlang(phase);
            setup(fluent);
            phases.Add(phase);
            return this;
        }

        /// <summary>
        /// Add a JavaScript map phase to the list of phases.
        /// </summary>
        /// <param name="setup">
        /// An <see cref="Action{T}"/> that accepts a <see cref="RiakFluentActionPhaseJavascript"/>,
        /// and configures the map phase with it.
        /// </param>
        /// <returns>A reference to this updated instance, for fluent chaining.</returns>
        /// <remarks>
        /// Configure the phase with a lambda similar to:
        /// <code>.MapJs(m => m.Source(@"function(o) {return [ 1 ];}"))</code>
        /// The above code will run a custom JavaScript function that returns "[1]" for each input in the phase.
        /// </remarks>
        public RiakMapReduceQuery MapJs(Action<RiakFluentActionPhaseJavascript> setup)
        {
            var phase = new RiakMapPhase<RiakPhaseLanguageJavascript>();
            var fluent = new RiakFluentActionPhaseJavascript(phase);
            setup(fluent);
            phases.Add(phase);
            return this;
        }

        /// <summary>
        /// Add an Erlang reduce phase to the list of phases.
        /// </summary>
        /// <param name="setup">
        /// An <see cref="Action{T}"/> that accepts a <see cref="RiakFluentActionPhaseErlang"/>,
        /// and configures the reduce phase with it.
        /// </param>
        /// <returns>A reference to this updated instance, for fluent chaining.</returns>
        /// <remarks>
        /// Configure the phase with a lambda similar to:
        /// <code>.ReduceErlang(r => r.ModFun("riak_kv_mapreduce", "reduce_set_union")</code>
        /// The code above will run the "riak_kv_mapreduce:reduce_set_union" Erlang function on the phase inputs.
        /// </remarks>
        public RiakMapReduceQuery ReduceErlang(Action<RiakFluentActionPhaseErlang> setup)
        {
            var phase = new RiakReducePhase<RiakPhaseLanguageErlang>();
            var fluent = new RiakFluentActionPhaseErlang(phase);
            setup(fluent);
            phases.Add(phase);
            return this;
        }

        /// <summary>
        /// Add a JavaScript reduce phase to the list of phases.
        /// </summary>
        /// <param name="setup">
        /// An <see cref="Action{T}"/> that accepts a <see cref="RiakFluentActionPhaseJavascript"/>,
        /// and configures the reduce phase with it.
        /// </param>
        /// <returns>A reference to this updated instance, for fluent chaining.</returns>
        /// <remarks>
        /// Configure the phase with a lambda similar to:
        /// <code>.ReduceJs(r => r.Name(@"Riak.reduceSum");</code>
        /// The code above will run the built-in JavaScript function "Riak.reduceSum" on the phase inputs.
        /// </remarks>
        public RiakMapReduceQuery ReduceJs(Action<RiakFluentActionPhaseJavascript> setup)
        {
            var phase = new RiakReducePhase<RiakPhaseLanguageJavascript>();
            var fluent = new RiakFluentActionPhaseJavascript(phase);
            setup(fluent);
            phases.Add(phase);
            return this;
        }

        /// <summary>
        /// Add a link phase to the list of phases.
        /// </summary>
        /// <param name="setup">
        /// An <see cref="Action{T}"/> that accepts a <see cref="RiakFluentLinkPhase"/>,
        /// and configures the link phase with it.
        /// </param>
        /// <returns>A reference to this updated instance, for fluent chaining.</returns>
        /// <remarks>
        /// Configure the phase with a lambda similar to:
        /// <code>.Link(l => l.Tag("friends");</code>
        /// The code above will walk links with a tag of "friends".
        /// </remarks>
        [Obsolete("Linkwalking is a deprecated feature of Riak and will eventually be removed.")]
        public RiakMapReduceQuery Link(Action<RiakFluentLinkPhase> setup)
        {
            var phase = new RiakLinkPhase();
            var fluent = new RiakFluentLinkPhase(phase);
            setup(fluent);
            phases.Add(phase);
            return this;
        }

#pragma warning disable 618
        /// <summary>
        /// Add a key filter input to the inputs.
        /// </summary>
        /// <param name="setup">
        /// An <see cref="Action{T}"/> that accepts a <see cref="RiakFluentKeyFilter"/>,
        /// and configures the input filter with it.
        /// </param>
        /// <returns>A reference to this updated instance, for fluent chaining.</returns>
        /// <remarks>
        /// Configure the phase with a lambda similar to:
        /// <code>new RiakMapReduceQuery().Inputs("Bucketname").Filter(f => f.StartsWith("time"));</code>
        /// The example above will filter out all keys that don't start with the string "time".
        /// Please see the <see cref="Fluent.RiakFluentKeyFilter"/> for
        /// more built-in filters.
        /// </remarks>
        [Obsolete("Key Filters are a deprecated feature of Riak and will eventually be removed.")]
        public RiakMapReduceQuery Filter(Action<RiakFluentKeyFilter> setup)
        {
            var filters = new List<IRiakKeyFilterToken>();
            var fluent = new RiakFluentKeyFilter(filters);
            setup(fluent);
            inputs.Filters.AddRange(filters);

            return this;
        }
#pragma warning restore 618

        /// <summary>
        /// Compile the mapreduce query.
        /// </summary>
        public void Compile()
        {
            Debug.Assert(inputs != null, "Compile inputs must not be null");

            if (!string.IsNullOrWhiteSpace(query))
            {
                return;
            }

            var sb = new StringBuilder();

            using (var sw = new StringWriter(sb))
            {
                using (JsonWriter writer = new JsonTextWriter(sw))
                {
                    writer.WriteStartObject();

                    if (inputs != null)
                    {
                        inputs.WriteJson(writer);
                    }

                    writer.WritePropertyName("query");

                    writer.WriteStartArray();
                    phases.ForEach(p => writer.WriteRawValue(p.ToJsonString()));
                    writer.WriteEndArray();

                    if (timeout.HasValue)
                    {
                        writer.WriteProperty<int>("timeout", timeout.Value);
                    }

                    writer.WriteEndObject();
                }
            }

            query = sb.ToString();
        }

        internal RpbMapRedReq ToMessage()
        {
            Compile();
            var message = new RpbMapRedReq
            {
                request = query.ToRiakString(),
                content_type = contentType.ToRiakString()
            };

            return message;
        }
    }
}
