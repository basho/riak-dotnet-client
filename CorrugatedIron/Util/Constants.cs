// Copyright (c) 2010 - OJ Reeves & Jeremiah Peschka
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

namespace CorrugatedIron.Util
{
    public static class Constants
    {
        public static class ContentTypes
        {
            public const string Any = @"*/*";
            public const string ApplicationOctetStream = @"application/octet-stream";
            public const string ApplicationJson = @"application/json";
            public const string TextPlain = @"text/plain";
            public const string TextHtml = @"text/html";
            public const string MultipartMixed = @"multipart/mixed";
            public const string ImageJpg = @"image/jpeg";
            public const string ImageGif = @"image/gif";
            public const string ImagePng = @"image/png";
            public const string ErlangBinary = @"application/x-erlang-binary";
        }
        
        public static class MapReduceLanguage
        {
            public const string JavaScript = "javascript";
            public const string Json = JavaScript;
            public const string Erlang = ContentTypes.ErlangBinary;
        }
        
        public static class MapReducePhaseType {
            public const string Map = @"map";
            public const string Reduce = @"reduce";
            public const string Link = @"link";
        }

        public static class CharSets
        {
            public const string Utf8 = @"UTF-8";
        }

        public const int ClientIdLength = 4;

        public static class Defaults
        {
            public const uint RVal = 2;
            public const string ContentType = ContentTypes.ApplicationOctetStream;
            public const string CharSet = CharSets.Utf8;
        }
        
        public static class KeyFilterTransforms
        {
            public const string IntToString = @"int_to_string";
            public const string StringToInt = @"string_to_int";
            public const string FloatToString = @"float_to_string";
            public const string StringToFloat = @"string_to_float";
            public const string ToUpper = @"to_upper";
            public const string ToLower = @"to_lower";
            public const string Tokenize = @"tokenize";
        }
    }
}
