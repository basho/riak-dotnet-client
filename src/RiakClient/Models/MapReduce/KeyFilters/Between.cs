// <copyright file="Between.cs" company="Basho Technologies, Inc.">
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
    using System;
    using System.IO;
    using System.Text;
    using Newtonsoft.Json;

    /// <summary>
    /// Tests that the input is between the first two arguments. 
    /// If the third argument is given, it is whether to treat the range as inclusive. 
    /// If the third argument is omitted, the range is treated as inclusive.
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    /// <remarks>It is assumed that left and right supply their own JSON conversion.</remarks>
    internal class Between<T> : IRiakKeyFilterToken
    {
        private readonly Tuple<string, T, T, bool> keyFilterDefinition;

        public Between(T left, T right, bool inclusive = true)
        {
            keyFilterDefinition = Tuple.Create("between", left, right, inclusive);
        }

        public string FunctionName
        {
            get { return keyFilterDefinition.Item1; }
        }

        public T Left
        {
            get { return keyFilterDefinition.Item2; }
        }

        public T Right
        {
            get { return keyFilterDefinition.Item3; }
        }

        public bool Inclusive
        {
            get { return keyFilterDefinition.Item4; }
        }

        public override string ToString()
        {
            return ToJsonString();
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
                jw.WriteValue(Left);
                jw.WriteValue(Right);
                jw.WriteValue(Inclusive);

                jw.WriteEndArray();
            }

            return sb.ToString();
        }
    }
}
