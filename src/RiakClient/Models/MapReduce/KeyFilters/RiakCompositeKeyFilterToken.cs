// <copyright file="RiakCompositeKeyFilterToken.cs" company="Basho Technologies, Inc.">
// Copyright 2011 - OJ Reeves & Jeremiah Peschka
// Copyright 2014 - Basho Technologies, Inc.
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

namespace RiakClient.Models.MapReduce.KeyFilters
{
    using System.Linq;
    using Newtonsoft.Json;

    internal abstract class RiakCompositeKeyFilterToken : RiakKeyFilterToken
    {
        protected RiakCompositeKeyFilterToken(string functionName, params object[] args)
            : base(functionName, args)
        {
        }

        protected override void WriteArguments(JsonWriter writer)
        {
            foreach (IRiakKeyFilterToken keyFilterToken in Arguments.Cast<IRiakKeyFilterToken>())
            {
                WriteArgumentAsArray(keyFilterToken, writer);
            }
        }

        protected void WriteArgumentAsArray(IRiakKeyFilterToken argument, JsonWriter writer)
        {
            /*
             * TODO: is StartArray really not needed? 
             * writer.WriteStartArray();
             */

            writer.WriteRawValue(argument.ToJsonString());

            // writer.WriteEndArray();
        }
    }
}
