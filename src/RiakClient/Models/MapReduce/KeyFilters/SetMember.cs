// <copyright file="SetMember.cs" company="Basho Technologies, Inc.">
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
    using Newtonsoft.Json;

    /// <summary>
    /// Tests that the input is contained in the set given as the arguments.
    /// </summary>
    /// <typeparam name="T">Type of key filter token</typeparam>
    internal class SetMember<T> : IRiakKeyFilterToken
    {
        private readonly Tuple<string, List<T>> keyFilterDefinition;

        public SetMember(List<T> set)
        {
            keyFilterDefinition = Tuple.Create("set_member", set);
        }

        public string FunctionName
        {
            get { return keyFilterDefinition.Item1; }
        }

        public List<T> Argument
        {
            get { return Set; }
        }

        public List<T> Set
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

            using (var sw = new StringWriter(sb))
            {
                using (JsonWriter jw = new JsonTextWriter(sw))
                {
                    jw.WriteStartArray();

                    jw.WriteValue(FunctionName);

                    Set.ForEach(v => jw.WriteValue(v));

                    jw.WriteEndArray();
                }
            }

            return sb.ToString();
        }
    }
}
