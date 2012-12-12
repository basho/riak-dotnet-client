// Copyright (c) 2011 - OJ Reeves & Jeremiah Peschka
// 
// This file is provided to you under the Apache License,
// Version 2.0 (the "License"); you may not use this file
// except in compliance with the License.  You may obtain
// a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing,
// software distributed under the License is distributed on an
// "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
// KIND, either express or implied.  See the License for the
// specific language governing permissions and limitations
// under the License.

using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;

namespace CorrugatedIron.Models.MapReduce.KeyFilters
{
    internal abstract class RiakKeyFilterToken : IRiakKeyFilterToken
    {
        private readonly Tuple<string, object, object> _kfDefinition;

        public string FunctionName
        {
            get { return _kfDefinition.Item1; }
        }

        public object[] Arguments
        {
            get { return new[] { _kfDefinition.Item2, _kfDefinition.Item3 }; }
        }

        protected RiakKeyFilterToken()
        {
        }

        protected RiakKeyFilterToken(string functionName, params object[] args)
        {
            _kfDefinition = new Tuple<string, object, object>(functionName, args[0], args[1]);
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

                WriteArguments(jw);

                jw.WriteEndArray();
            }

            return sb.ToString();
        }

        protected virtual void WriteArguments(JsonWriter writer)
        {
            writer.WriteValue(_kfDefinition.Item2);
            writer.WriteValue(_kfDefinition.Item3);
        }
    }
}