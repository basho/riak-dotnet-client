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

using System.Collections.Generic;
using CorrugatedIron.Extensions;
using CorrugatedIron.Models.MapReduce.KeyFilters;
using Newtonsoft.Json;

namespace CorrugatedIron.Models.MapReduce.Inputs
{
    public class RiakModuleFunctionArgInput : RiakPhaseInput
    {
        public string Module { get; set; }
        public string Function { get; set; }
        public string[] Arg { get; set; }
        
        public RiakModuleFunctionArgInput()
        {
        }
        
        public RiakModuleFunctionArgInput(string module, string function, string[] arg)
        {
            Module = module;
            Function = function;
            Arg = arg;
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
            Arg.ForEach(a => writer.WriteValue(a));
            writer.WriteEndArray();
            
            writer.WriteEndObject();

            return writer;
        }
    }
}
