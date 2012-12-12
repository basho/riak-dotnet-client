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
using System.IO;
using System.Text;

namespace CorrugatedIron.Models.MapReduce.KeyFilters
{
    /// <summary>
    /// Turns an integer (previously extracted with string_to_int), into a string.
    /// </summary>
    internal class IntToString : IRiakKeyFilterToken
    {
        public string FunctionName
        {
            get { return "int_to_string"; }
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

                jw.WriteEndArray();
            }

            return sb.ToString();
        }
    }
}