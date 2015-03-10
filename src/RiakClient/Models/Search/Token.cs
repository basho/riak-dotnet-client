// <copyright file="Token.cs" company="Basho Technologies, Inc.">
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

namespace RiakClient.Models.Search
{
    using System.Text.RegularExpressions;

    /// <summary>
    /// Represents a Lucene search token.
    /// </summary>
    public class Token
    {
        private static readonly Regex EncodeRegex = new Regex(@"(["" \\'\(\)\[\]\\:\+\-\/\?])");

        private readonly string value;
        private readonly string suffix;

        internal Token(string value)
            : this(value, null)
        {
        }

        internal Token(string value, string suffix)
        {
            this.value = value;
            this.suffix = suffix;
        }

        /// <summary>
        /// Create a token for searching for field values that exactly match the parameter <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The value to search for.</param>
        /// <returns>A newly initialized and configured <see cref="Token"/>.</returns>
        public static Token Is(string value)
        {
            return new Token(value);
        }

        /// <summary>
        /// Create a token for searching for field values that start with the parameter <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The value to search for.</param>
        /// <returns>A newly initialized and configured <see cref="Token"/>.</returns>
        public static Token StartsWith(string value)
        {
            return new Token(value, "*");
        }

        /// <summary>
        /// Returns the token in a format acceptable for Lucene query strings.
        /// </summary>
        /// <returns>A string that represents the token.</returns>
        public override string ToString()
        {
            return value != null ? EncodeRegex.Replace(value, m => "\\" + m.Value) + suffix : string.Empty;
        }
    }
}
