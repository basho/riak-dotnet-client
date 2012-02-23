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

using System.Web;
using System.Text.RegularExpressions;
using CorrugatedIron.Util;

namespace CorrugatedIron.Extensions
{
    public static class StringExtensions
    {
        private static readonly System.Text.Encoding RiakEncoding = System.Text.Encoding.UTF8;

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
            return string.Format(formatter, args);
        }

        public static string UrlEncoded(this string value)
        {
            return HttpUtility.UrlEncode(value);
        }

        public static bool IsIntegerKey(this string value)
        {
            return value.EndsWith(RiakConstants.IndexSuffix.Integer);
        }

        public static bool IsBinaryKey(this string value)
        {
            return value.EndsWith(RiakConstants.IndexSuffix.Binary);
        }

        public static string ToIntegerKey(this string value)
        {
            return value.IsIntegerKey() ? value : value + RiakConstants.IndexSuffix.Integer;
        }

        public static string ToBinaryKey(this string value)
        {
            return value.IsBinaryKey() ? value : value + RiakConstants.IndexSuffix.Binary;
        }
        
        public static string ToRiakSearchTerm(this string value) 
        {
            // + - && || ! ( ) { } [ ] ^ " ~ * ? : \
            const string pattern = @"[\+\-!\(\)\{\}\[\]^\""~\*\?\:\\]{1}";
            
            const string replacement = @"\$&";
            
            var regex = new Regex(pattern);
            var result = regex.Replace(value, replacement);

            // if this is a range query, we can skip the double quotes
            var valueLength = value.Length;
            if ((value[0] == '[' && value[valueLength - 1] == ']')
                || (value[0] == '{' && value[valueLength - 1] == '}'))
            {
                return result;
            }

            // If we have a phrase, then we want to put double quotes around the Term
            if (value.IndexOf(" ") > -1) {
                result = string.Format("\"{0}\"", result);
            }
            
            return result;
        }
    }
}
