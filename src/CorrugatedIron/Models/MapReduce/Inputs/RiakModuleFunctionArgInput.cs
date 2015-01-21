// Copyright (c) 2011 - 2014 OJ Reeves & Jeremiah Peschka
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

using CorrugatedIron.Extensions;
using Newtonsoft.Json;

namespace CorrugatedIron.Models.MapReduce.Inputs
{
    public class RiakModuleFunctionArgInput : RiakPhaseInput
    {
        private readonly string[] _arg;
        private readonly string _function;
        private readonly string _module;

        public RiakModuleFunctionArgInput(string module, string function, string[] arg)
        {
            _module = module;
            _function = function;
            _arg = arg;
        }

        public string Module
        {
            get { return _module; }
        }

        public string Function
        {
            get { return _function; }
        }

        public string[] Arg
        {
            get { return _arg; }
        }

        public override JsonWriter WriteJson(JsonWriter writer)
        {
            writer.WritePropertyName("inputs");
            writer.WriteStartObject();

            writer.WritePropertyName("module");
            writer.WriteValue(Module);

            writer.WritePropertyName("function");
            writer.WriteValue(Function);

            writer.WritePropertyName("arg");

            writer.WriteStartArray();
            Arg.ForEach(writer.WriteValue);
            writer.WriteEndArray();

            writer.WriteEndObject();

            return writer;
        }
    }
}