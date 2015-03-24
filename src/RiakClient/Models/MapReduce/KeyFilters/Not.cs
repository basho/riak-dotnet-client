// <copyright file="Not.cs" company="Basho Technologies, Inc.">
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

namespace RiakClient.Models.MapReduce.KeyFilters
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using Extensions;
    using Newtonsoft.Json;

    /// <summary>
    /// Negates the result of key-filter operations.
    /// </summary>
    internal class Not : IRiakKeyFilterToken
    {
        private readonly Tuple<string, List<IRiakKeyFilterToken>> keyFilterDefinition;

        public Not(List<IRiakKeyFilterToken> arg)
        {
            keyFilterDefinition = Tuple.Create("not", arg);
        }

        public string FunctionName
        {
            get { return keyFilterDefinition.Item1; }
        }

        public List<IRiakKeyFilterToken> Argument
        {
            get { return keyFilterDefinition.Item2; }
        }

        public override string ToString()
        {
            return ToJsonString();
        }

        public string ToJsonString()
        {
            var sb = new StringBuilder();
            var sw = new StringWriter(sb);
            
            using (JsonWriter jw = new JsonTextWriter(sw))
            {
                jw.WriteStartArray();

                jw.WriteValue(FunctionName);

                jw.WriteRawFilterTokenArray(Argument);

                jw.WriteEndArray();
            }

            return sb.ToString();
        }
    }
}
