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
    using System.IO;
    using System.Text;
    using Extensions;
    using Messages;
    using Models.MapReduce.Fluent;
    using Models.MapReduce.Inputs;
    using Models.MapReduce.KeyFilters;
    using Models.MapReduce.Languages;
    using Models.MapReduce.Phases;
    using Newtonsoft.Json;
    using Util;

    public class RiakMapReduceQuery
    {
        private readonly List<RiakPhase> phases = new List<RiakPhase>();
        private readonly string contentType = RiakConstants.ContentTypes.ApplicationJson;
        private readonly int? timeout = null;

        private string query;
        private RiakPhaseInput inputs;

        public RiakMapReduceQuery() : this(RiakConstants.ContentTypes.ApplicationJson, null)
        {
        }

        public RiakMapReduceQuery(string contentType, int? timeout = null)
        {
            if (string.IsNullOrWhiteSpace(contentType))
            {
                throw new ArgumentNullException("contentType");
            }

            this.contentType = contentType;
            this.timeout = timeout;
        }

        public string ContentType
        {
            get { return contentType; }
        }

        public int? Timeout
        {
            get { return timeout; }
        }

        public RiakMapReduceQuery Inputs(string bucket)
        {
            inputs = new RiakBucketInput(bucket);
            return this;
        }

        // TODO: Replace the backwardness of these parameters when we get a Namespace class.
        public RiakMapReduceQuery Inputs(string bucket, string bucketType)
        {
            inputs = new RiakBucketInput(bucket, bucketType);
            return this;
        }

        [Obsolete("Using Legacy Search as input for MapReduce is depreciated. Please move to Riak 2.0 Search, and use the RiakSearchInput class instead.")]
        public RiakMapReduceQuery Inputs(RiakBucketSearchInput riakBucketSearchInput)
        {
            inputs = riakBucketSearchInput;
            return this;
        }

        public RiakMapReduceQuery Inputs(RiakSearchInput riakSearchInput)
        {
            inputs = riakSearchInput;
            return this;
        }

        public RiakMapReduceQuery Inputs(RiakBucketKeyInput riakBucketKeyInputs)
        {
            inputs = riakBucketKeyInputs;
            return this;
        }

        public RiakMapReduceQuery Inputs(RiakBucketKeyKeyDataInput riakBucketKeyKeyDataInputs)
        {
            inputs = riakBucketKeyKeyDataInputs;
            return this;
        }

        public RiakMapReduceQuery Inputs(RiakModuleFunctionArgInput riakModFunArgsInput)
        {
            inputs = riakModFunArgsInput;
            return this;
        }

        public RiakMapReduceQuery Inputs(RiakIndexInput riakIndexPhaseInput)
        {
            inputs = riakIndexPhaseInput;
            return this;
        }

        public RiakMapReduceQuery MapErlang(Action<RiakFluentActionPhaseErlang> setup)
        {
            var phase = new RiakMapPhase<RiakPhaseLanguageErlang>();
            var fluent = new RiakFluentActionPhaseErlang(phase);
            setup(fluent);
            phases.Add(phase);
            return this;
        }

        public RiakMapReduceQuery MapJs(Action<RiakFluentActionPhaseJavascript> setup)
        {
            var phase = new RiakMapPhase<RiakPhaseLanguageJavascript>();
            var fluent = new RiakFluentActionPhaseJavascript(phase);
            setup(fluent);
            phases.Add(phase);
            return this;
        }

        public RiakMapReduceQuery ReduceErlang(Action<RiakFluentActionPhaseErlang> setup)
        {
            var phase = new RiakReducePhase<RiakPhaseLanguageErlang>();
            var fluent = new RiakFluentActionPhaseErlang(phase);
            setup(fluent);
            phases.Add(phase);
            return this;
        }

        public RiakMapReduceQuery ReduceJs(Action<RiakFluentActionPhaseJavascript> setup)
        {
            var phase = new RiakReducePhase<RiakPhaseLanguageJavascript>();
            var fluent = new RiakFluentActionPhaseJavascript(phase);
            setup(fluent);
            phases.Add(phase);
            return this;
        }

        public RiakMapReduceQuery Link(Action<RiakFluentLinkPhase> setup)
        {
            var phase = new RiakLinkPhase();
            var fluent = new RiakFluentLinkPhase(phase);
            setup(fluent);
            phases.Add(phase);
            return this;
        }

        public RiakMapReduceQuery Filter(Action<RiakFluentKeyFilter> setup)
        {
            var filters = new List<IRiakKeyFilterToken>();
            var fluent = new RiakFluentKeyFilter(filters);
            setup(fluent);
            inputs.Filters.AddRange(filters);

            return this;
        }

        public void Compile()
        {
            System.Diagnostics.Debug.Assert(inputs != null, "Compile inputs must not be null");

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
