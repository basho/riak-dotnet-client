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

using CorrugatedIron.Extensions;
using CorrugatedIron.Messages;
using CorrugatedIron.Models.MapReduce.Fluent;
using CorrugatedIron.Models.MapReduce.Inputs;
using CorrugatedIron.Models.MapReduce.KeyFilters;
using CorrugatedIron.Models.MapReduce.Languages;
using CorrugatedIron.Models.MapReduce.Phases;
using CorrugatedIron.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CorrugatedIron.Models.MapReduce
{
    public class RiakMapReduceQuery
    {
        private readonly List<RiakPhase> _phases;

        private string _query;
        private RiakPhaseInput _inputs;

        public string ContentType { get; set; }

        public int? Timeout { get; set; }

        public RiakMapReduceQuery()
        {
            _phases = new List<RiakPhase>();
            ContentType = RiakConstants.ContentTypes.ApplicationJson;
        }

        public RiakMapReduceQuery Inputs(string bucket)
        {
            _inputs = new RiakBucketInput(bucket);
            return this;
        }

        public RiakMapReduceQuery Inputs(RiakBucketSearchInput riakBucketSearchInput)
        {
            _inputs = riakBucketSearchInput;
            return this;
        }

        public RiakMapReduceQuery Inputs(RiakBucketKeyInput riakBucketKeyInputs)
        {
            _inputs = riakBucketKeyInputs;
            return this;
        }

        public RiakMapReduceQuery Inputs(RiakBucketKeyKeyDataInput riakBucketKeyKeyDataInputs)
        {
            _inputs = riakBucketKeyKeyDataInputs;
            return this;
        }


        public RiakMapReduceQuery Inputs(RiakModuleFunctionArgInput riakModFunArgsInput)
        {
            _inputs = riakModFunArgsInput;
            return this;
        }

        public RiakMapReduceQuery Inputs(RiakIndexInput riakIndexPhaseInput)
        {
            _inputs = riakIndexPhaseInput;
            return this;
        }

        public RiakMapReduceQuery MapErlang(Action<RiakFluentActionPhaseErlang> setup)
        {
            var phase = new RiakMapPhase<RiakPhaseLanguageErlang>();
            var fluent = new RiakFluentActionPhaseErlang(phase);
            setup(fluent);
            _phases.Add(phase);
            return this;
        }

        public RiakMapReduceQuery MapJs(Action<RiakFluentActionPhaseJavascript> setup)
        {
            var phase = new RiakMapPhase<RiakPhaseLanguageJavascript>();
            var fluent = new RiakFluentActionPhaseJavascript(phase);
            setup(fluent);
            _phases.Add(phase);
            return this;
        }

        public RiakMapReduceQuery ReduceErlang(Action<RiakFluentActionPhaseErlang> setup)
        {
            var phase = new RiakReducePhase<RiakPhaseLanguageErlang>();
            var fluent = new RiakFluentActionPhaseErlang(phase);
            setup(fluent);
            _phases.Add(phase);
            return this;
        }

        public RiakMapReduceQuery ReduceJs(Action<RiakFluentActionPhaseJavascript> setup)
        {
            var phase = new RiakReducePhase<RiakPhaseLanguageJavascript>();
            var fluent = new RiakFluentActionPhaseJavascript(phase);
            setup(fluent);
            _phases.Add(phase);
            return this;
        }

        public RiakMapReduceQuery Link(Action<RiakFluentLinkPhase> setup)
        {
            var phase = new RiakLinkPhase();
            var fluent = new RiakFluentLinkPhase(phase);
            setup(fluent);
            _phases.Add(phase);
            return this;
        }

        public RiakMapReduceQuery Filter(Action<RiakFluentKeyFilter> setup)
        {
            var filters = new List<IRiakKeyFilterToken>();
            var fluent = new RiakFluentKeyFilter(filters);
            setup(fluent);
            _inputs.Filters.AddRange(filters);

            return this;
        }

        public void Compile()
        {
            System.Diagnostics.Debug.Assert(_inputs != null);
            if(!string.IsNullOrWhiteSpace(_query))
            {
                return;
            }

            var sb = new StringBuilder();

            using(var sw = new StringWriter(sb))
            using(JsonWriter writer = new JsonTextWriter(sw))
            {
                writer.WriteStartObject();

                if(_inputs != null)
                {
                    _inputs.WriteJson(writer);
                }

                writer.WritePropertyName("query");

                writer.WriteStartArray();
                _phases.ForEach(p => writer.WriteRawValue(p.ToJsonString()));
                writer.WriteEndArray();

                if (Timeout.HasValue)
                {
                    writer.WriteProperty<int>("timeout", Timeout.Value);
                }

                writer.WriteEndObject();
            }

            _query = sb.ToString();
        }

        internal RpbMapRedReq ToMessage()
        {
            Compile();
            var message = new RpbMapRedReq
            {
                request = _query.ToRiakString(),
                content_type = ContentType.ToRiakString()
            };

            return message;
        }
    }
}