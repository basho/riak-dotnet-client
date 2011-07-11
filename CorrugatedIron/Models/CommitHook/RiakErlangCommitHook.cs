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
using Newtonsoft.Json;

namespace CorrugatedIron.Models.CommitHook
{
    public class RiakErlangCommitHook : RiakCommitHook, IRiakPreCommitHook, IRiakPostCommitHook
    {
        public string Module { get; private set; }
        public string Function { get; private set; }

        public RiakErlangCommitHook(string module, string function)
        {
            Module = module;
            Function = function;
        }

        public override void WriteJson(JsonWriter writer)
        {
            writer.WriteStartObject();
            writer.WriteProperty("mod", Module);
            writer.WriteProperty("fun", Function);
            writer.WriteEndObject();
        }
    }
}
