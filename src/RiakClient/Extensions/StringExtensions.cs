// <copyright file="StringExtensions.cs" company="Basho Technologies, Inc.">
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

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using RiakClient.Util;

namespace RiakClient.Extensions
{
    public static class StringExtensions
    {
        private static readonly Encoding RiakEncoding = new UTF8Encoding(false);

        // + - && || ! ( ) { } [ ] ^ " ~ * ? : \
        private const string SearchTermPattern = @"[\+\-!\(\)\{\}\[\]^\""~\*\?\:\\]{1}";
        private const string SearchTermReplacement = @"\$&";
        private static readonly Regex SearchTermRegex = new Regex(SearchTermPattern, RegexOptions.Compiled);

        public static byte[] ToRiakString(this string value)
        {
            return value == null ? null : RiakEncoding.GetBytes(value);
        }

        public static string FromRiakString(this byte[] value)
        {
            return value == null ? null : RiakEncoding.GetString(value);
        }

        public static string Fmt(this string formatter, params object[] args)
        {
            return String.Format(formatter, args);
        }

        public static string UrlEncoded(this string value)
        {
            return HttpUtility.UrlEncode(value);
        }

        public static bool IsUserIntegerKey(this string value)
        {
            return RiakConstants.SystemIndexKeys.SystemIntKeys.Contains(value)
                || value.EndsWith(RiakConstants.IndexSuffix.Integer);
        }

        public static bool IsUserBinaryKey(this string value)
        {
            return RiakConstants.SystemIndexKeys.SystemBinKeys.Contains(value)
                || value.EndsWith(RiakConstants.IndexSuffix.Binary);
        }

        public static bool IsSystemIntegerKey(this string value)
        {
            return RiakConstants.SystemIndexKeys.SystemIntKeys.Contains(value);
        }

        public static bool IsSystemBinaryKey(this string value)
        {
            return RiakConstants.SystemIndexKeys.SystemBinKeys.Contains(value);
        }

        public static bool IsSystemKey(this string value)
        {
            return RiakConstants.SystemIndexKeys.SystemBinKeys.Contains(value)
                || RiakConstants.SystemIndexKeys.SystemIntKeys.Contains(value);
        }

        public static string ToIntegerKey(this string value)
        {
            return value.IsSystemIntegerKey() ? value : value + RiakConstants.IndexSuffix.Integer;
        }

        public static string ToBinaryKey(this string value)
        {
            return value.IsSystemBinaryKey() ? value : value + RiakConstants.IndexSuffix.Binary;
        }

        public static string ToRiakSearchTerm(this string value)
        {
            var result = SearchTermRegex.Replace(value, SearchTermReplacement);

            // if this is a range query, we can skip the double quotes
            var valueLength = value.Length;
            if ((value[0] == '[' && value[valueLength - 1] == ']')
                || (value[0] == '{' && value[valueLength - 1] == '}'))
            {
                return result;
            }

            // If we have a phrase, then we want to put double quotes around the Term
            if (value.IndexOf(" ") > -1)
            {
                result = string.Format("\"{0}\"", result);
            }

            return result;
        }

        internal static uint ToRpbOption(this string value)
        {
            System.Diagnostics.Debug.Assert(new HashSet<string> { "all", "quorum", "one", "default" }.Contains(value), "Incorrect quorum value");

            return RiakConstants.QuorumOptionsLookup[value];
        }
    }
}
