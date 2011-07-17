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
using CorrugatedIron.Util;
using Newtonsoft.Json;

namespace CorrugatedIron.Models.MapReduce.Languages
{
    internal class RiakPhaseLanguageErlang : IRiakPhaseLanguage
    {
        private string _module;
        private string _function;

        public void ModFun(string module, string function)
        {
            _module = module;
            _function = function;
        }

        public void WriteJson(JsonWriter writer)
        {
            System.Diagnostics.Debug.Assert(!string.IsNullOrWhiteSpace(_module), "Module must be set");
            System.Diagnostics.Debug.Assert(!string.IsNullOrWhiteSpace(_function), "Function must be set");

            writer.WriteSpecifiedProperty("language", RiakConstants.MapReduceLanguage.Erlang)
                .WriteSpecifiedProperty("module", _module)
                .WriteSpecifiedProperty("function", _function);
        }
    }
}