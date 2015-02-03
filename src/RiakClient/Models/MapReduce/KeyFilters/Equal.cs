// <copyright file="Equal.cs" company="Basho Technologies, Inc.">
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
    using System.IO;
    using System.Text;
    using Newtonsoft.Json;

    /// <summary>
    /// Tests that the input is equal to the argument.
    /// </summary>
    /// <typeparam name="T">Type of key filter token</typeparam>
    internal class Equal<T> : IRiakKeyFilterToken
    {
        private readonly Tuple<string, T> keyFilterDefinition;

        public Equal(T arg)
        {
            keyFilterDefinition = new Tuple<string, T>("eq", arg);
        }

        public string FunctionName
        {
            get { return keyFilterDefinition.Item1; }
        }

        public T Argument
        {
            get { return keyFilterDefinition.Item2; }
        }

        public string ToJsonString()
        {
            /*
             * NB: JsonTextWriter is guaranteed to close the StringWriter
             * https://github.com/JamesNK/Newtonsoft.Json/blob/master/Src/Newtonsoft.Json/JsonTextWriter.cs#L150-L160
             */
            var sb = new StringBuilder();
            var sw = new StringWriter(sb);
            using (JsonWriter jw = new JsonTextWriter(sw))
            {
                jw.WriteStartArray();

                jw.WriteValue(FunctionName);
                jw.WriteValue(Argument);

                jw.WriteEndArray();
            }

            return sb.ToString();
        }

        public override string ToString()
        {
            return ToJsonString();
        }
    }
}
