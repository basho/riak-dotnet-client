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

using CorrugatedIron.Models.MapReduce.KeyFilters;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace CorrugatedIron.Models.MapReduce.Inputs
{
    public interface IRiakPhaseInput
    {
        JsonWriter WriteJson(JsonWriter writer);
    }

    public abstract class RiakPhaseInput : IRiakPhaseInput
    {
        public List<IRiakKeyFilterToken> Filters { get; set; }
        public abstract JsonWriter WriteJson(JsonWriter writer);
    }

    // TODO: Confirm with JP that this isn't violating LSP.
    public abstract class RiakIndexInput : RiakPhaseInput
    {
    }
}
