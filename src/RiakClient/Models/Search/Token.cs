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

using System.Text.RegularExpressions;

namespace CorrugatedIron.Models.Search
{
    public class Token
    {
        private static readonly Regex EncodeRegex = new Regex(@"(["" \\'\(\)\[\]\\:\+\-\/\?])");

        private readonly string _value;
        private readonly string _suffix;

        internal Token(string value)
            : this(value, null)
        {
        }

        internal Token(string value, string suffix)
        {
            _value = value;
            _suffix = suffix;
        }

        public static Token Is(string value)
        {
            return new Token(value);
        }

        public static Token StartsWith(string value)
        {
            return new Token(value, "*");
        }

        public override string ToString()
        {
            return _value != null ? EncodeRegex.Replace(_value, m => "\\" + m.Value) + _suffix : string.Empty;
        }
    }
}