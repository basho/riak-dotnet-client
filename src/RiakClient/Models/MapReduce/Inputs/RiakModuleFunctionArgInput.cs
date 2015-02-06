// <copyright file="RiakModuleFunctionArgInput.cs" company="Basho Technologies, Inc.">
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
    using Newtonsoft.Json;

    public class RiakModuleFunctionArgInput : RiakPhaseInput
    {
        private readonly string[] arg;
        private readonly string function;
        private readonly string module;

        public RiakModuleFunctionArgInput(string module, string function, string[] arg)
        {
            this.module = module;
            this.function = function;
            this.arg = arg;
        }

        public string Module
        {
            get { return module; }
        }

        public string Function
        {
            get { return function; }
        }

        public string[] Arg
        {
            get { return arg; }
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

            foreach (string arg in Arg)
            {
                writer.WriteValue(arg);
            }

            writer.WriteEndArray();

            writer.WriteEndObject();

            return writer;
        }
    }
}
