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

using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;

namespace CorrugatedIron.Models.MapReduce.KeyFilters
{
    /// <summary>
    /// Tests that the input is between the first two arguments. 
    /// If the third argument is given, it is whether to treat the range as inclusive. 
    /// If the third argument is omitted, the range is treated as inclusive.
    /// </summary>
    /// <remarks>It is assumed that left and right supply their own JSON conversion.</remarks>
    internal class Between<T> : IRiakKeyFilterToken
    {
        private readonly Tuple<string, T, T, bool> _kfDefinition;

        public string FunctionName
        {
            get { return _kfDefinition.Item1; }
        }

        public T Left
        {
            get { return _kfDefinition.Item2; }
        }

        public T Right
        {
            get { return _kfDefinition.Item3; }
        }

        public bool Inclusive
        {
            get { return _kfDefinition.Item4; }
        }

        public Between(T left, T right, bool inclusive = true)
        {
            _kfDefinition = Tuple.Create("between", left, right, inclusive);
        }

        public override string ToString()
        {
            return ToJsonString();
        }

        public string ToJsonString()
        {
            var sb = new StringBuilder();

            using(var sw = new StringWriter(sb))
            using(JsonWriter jw = new JsonTextWriter(sw))
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
