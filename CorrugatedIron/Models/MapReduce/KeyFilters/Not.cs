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

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CorrugatedIron.Models.MapReduce.KeyFilters
{
    /// <summary>
    /// Negates the result of key-filter operations.
    /// </summary>
    internal class Not : IRiakKeyFilterToken
    {
        private readonly Tuple<string, List<IRiakKeyFilterToken>> _kfDefinition;

        public string FunctionName
        {
            get { return _kfDefinition.Item1; }
        }

        public List<IRiakKeyFilterToken> Argument
        {
            get { return _kfDefinition.Item2; }
        }

        public Not(List<IRiakKeyFilterToken> arg)
        {
            _kfDefinition = Tuple.Create("not", arg);
        }

        public override string ToString()
        {
            return ToJsonString();
        }

        public string ToJsonString()
        {
            var sb = new StringBuilder();

            using(var sw = new StringWriter(sb))
            using(JsonWriter jw = new JsonTextWriter(sw))
            {
                jw.WriteStartArray();

                jw.WriteValue(FunctionName);
                jw.WriteStartArray();

                Argument.ForEach(a => jw.WriteRawValue(a.ToString()));

                jw.WriteEndArray();
                jw.WriteEndArray();
            }

            return sb.ToString();
        }
    }
}