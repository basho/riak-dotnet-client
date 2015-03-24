// <copyright file="And.cs" company="Basho Technologies, Inc.">
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
    /// Joins two or more key-filter operations with a logical AND operation.
    /// </summary>
    internal class And : IRiakKeyFilterToken
    {
        /// <summary>
        ///  Collection of key filter definitions.
        /// </summary>
        private readonly Tuple<string, List<IRiakKeyFilterToken>, List<IRiakKeyFilterToken>> keyFilterDefinition;

        /// <summary>
        /// Initializes a new instance of the <see cref="And"/> class.
        /// </summary>
        /// <param name="left">left hand token</param>
        /// <param name="right">right hand token</param>
        public And(List<IRiakKeyFilterToken> left, List<IRiakKeyFilterToken> right)
        {
            keyFilterDefinition = Tuple.Create("and", left, right);
        }

        /// <summary>
        /// Gets the function name
        /// </summary>
        public string FunctionName
        {
            get { return keyFilterDefinition.Item1; }
        }

        /// <summary>
        /// Gets the left hand token
        /// </summary>
        public List<IRiakKeyFilterToken> Left
        {
            get { return keyFilterDefinition.Item2; }
        }

        /// <summary>
        /// Gets the right hand token
        /// </summary>
        public List<IRiakKeyFilterToken> Right
        {
            get { return keyFilterDefinition.Item3; }
        }

        /// <summary>
        /// Converts the current token to JSON
        /// </summary>
        /// <returns>JSON representation of the <see cref="And"/> class</returns>
        public override string ToString()
        {
            return ToJsonString();
        }

        /// <summary>
        /// Converts the current token to JSON
        /// </summary>
        /// <returns>JSON representation of the <see cref="And"/> class</returns>
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

                jw.WriteRawFilterTokenArray(Left);

                jw.WriteRawFilterTokenArray(Right);

                jw.WriteEndArray();
            }
            
            return sb.ToString();
        }
    }
}
