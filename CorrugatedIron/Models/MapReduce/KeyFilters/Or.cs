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

using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace CorrugatedIron.Models.MapReduce.KeyFilters
{
    /// <summary>
    /// Joins two or more key-filter operations with a logical OR operation.
    /// </summary>
    public class Or : IRiakKeyFilterToken
    {
        private Tuple<string, IRiakKeyFilterToken, IRiakKeyFilterToken> _kfDefinition;

        public string FunctionName
        {
            get { return _kfDefinition.Item1; }
        }

        public IRiakKeyFilterToken Left
        {
            get { return _kfDefinition.Item2; }
        }

        public IRiakKeyFilterToken Right
        {
            get { return _kfDefinition.Item3; }
        }

        public Or(IRiakKeyFilterToken left, IRiakKeyFilterToken right)
        {
            _kfDefinition = new Tuple<string, IRiakKeyFilterToken, IRiakKeyFilterToken>("or", left, right);
        }

        public override string ToString()
        {
            return ToJsonString();
        }

        public string ToJsonString()
        {
            var sb = new StringBuilder();

            using (var sw = new StringWriter(sb))
            using (JsonWriter jw = new JsonTextWriter(sw))
            {
                jw.WriteStartArray();

                jw.WriteValue(FunctionName);

                jw.WriteStartArray();
                jw.WriteRawValue(Left.ToString());
                jw.WriteEndArray();

                jw.WriteStartArray();
                jw.WriteRawValue(Right.ToString());
                jw.WriteEndArray();

                jw.WriteEndArray();
            }

            return sb.ToString();
        }
    }
}