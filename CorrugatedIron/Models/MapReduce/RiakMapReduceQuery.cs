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
using CorrugatedIron.Models.MapReduce.Inputs;
using CorrugatedIron.Util;
using Newtonsoft.Json;

namespace CorrugatedIron.Models.MapReduce
{
    public class RiakMapReduceQuery
    {
        private readonly List<RiakPhase> _phases;
        private readonly List<IRiakKeyFilterToken> _filters;
        private string _query;
        private RiakPhaseInputs _inputs;

        public string ContentType { get; set; }

        public RiakMapReduceQuery()
        {
            _phases = new List<RiakPhase>();
            _filters = new List<IRiakKeyFilterToken>();
            ContentType = Constants.ContentTypes.ApplicationJson;
        }

        public RiakMapReduceQuery Inputs(RiakPhaseInputs inputs)
        {
            _inputs = inputs;
            return this;
        }

        public RiakMapReduceQuery Map(Action<RiakMapPhase> setup)
        {
            return Phase(setup);
        }

        public RiakMapReduceQuery Reduce(Action<RiakReducePhase> setup)
        {
            return Phase(setup);
        }

        public RiakMapReduceQuery Link(Action<RiakLinkPhase> setup)
        {
            return Phase(setup);
        }

        public RiakMapReduceQuery Filter(IRiakKeyFilterToken filter)
        {
            _filters.Add(filter);
            return this;
        }

        private RiakMapReduceQuery Phase<TPhase>(Action<TPhase> setup)
            where TPhase : RiakPhase, new()
        {
            var phase = new TPhase();
            setup(phase);
            _phases.Add(phase);
            return this;
        }

        public void Compile()
        {
            System.Diagnostics.Debug.Assert(_inputs != null);
            if (!string.IsNullOrWhiteSpace(_query)) return;

            var sb = new StringBuilder();

            using (var sw = new StringWriter(sb))
            using(JsonWriter writer = new JsonTextWriter(sw))
            {
                writer.WriteStartObject();

                writer.WritePropertyName("inputs");
                _inputs.WriteJson(writer);

                if (_filters.Count > 0)
                {
                    writer.WritePropertyName("key_filters");
                    writer.WriteStartArray();
                    _filters.ForEach(f => writer.WriteRawValue(f.ToJsonString()));
                    writer.WriteEndArray();
                }

                writer.WritePropertyName("query");
                writer.WriteStartArray();
                _phases.ForEach(p => writer.WriteRawValue(p.ToJsonString()));
                writer.WriteEndArray();

                writer.WriteEndObject();
            }

            _query = sb.ToString();
        }

        internal RpbMapRedReq ToMessage()
        {
            Compile();
            var message = new RpbMapRedReq
                              {
                                  Request = _query.ToRiakString(),
                                  ContentType = ContentType.ToRiakString()
                              };

            return message;
        }
    }
}
