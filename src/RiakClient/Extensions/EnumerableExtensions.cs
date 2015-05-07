// <copyright file="EnumerableExtensions.cs" company="Basho Technologies, Inc.">
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

namespace RiakClient.Extensions
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    internal static class EnumerableExtensions
    {
        private static readonly UTF8Encoding UTF8 = new UTF8Encoding();

        public static IEnumerable<byte[]> GetUTF8Bytes(this IEnumerable<string> strings)
        {
            return GetBytes(strings, UTF8);
        }

        public static IEnumerable<byte[]> GetBytes(this IEnumerable<string> strings, Encoding encoding)
        {
            IEnumerable<byte[]> rv = null;

            if (strings != null)
            {
                rv = strings.Select(encoding.GetBytes);
            }

            return rv;
        }

        public static IEnumerable<string> GetUTF8Strings(this IEnumerable<byte[]> bytes)
        {
            return GetStrings(bytes, UTF8);
        }

        public static IEnumerable<string> GetStrings(this IEnumerable<byte[]> bytes, Encoding encoding)
        {
            IEnumerable<string> rv = null;

            if (bytes != null)
            {
                rv = bytes.Select(encoding.GetString);
            }

            return rv;
        }
    }
}