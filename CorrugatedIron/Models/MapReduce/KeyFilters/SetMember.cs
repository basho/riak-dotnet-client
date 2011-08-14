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
using Newtonsoft.Json;

namespace CorrugatedIron.Models.MapReduce.KeyFilters
{
    /// <summary>
    /// Tests that the input is contained in the set given as the arguments.
    /// </summary>
    public class SetMember<T> : RiakKeyFilterToken
    {
        public List<T> Set { get; private set; }

        public SetMember(List<T> set)
            : base("set_member", set.ToArray())
        {
            Set = set;
        }

        protected override void WriteArguments(object[] arguments, JsonWriter writer)
        {
            Set.ForEach(v => writer.WriteValue(v));
        }
    }
}