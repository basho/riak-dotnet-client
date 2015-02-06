// <copyright file="RiakPhaseLanguageJavascript.cs" company="Basho Technologies, Inc.">
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

namespace RiakClient.Models.MapReduce.Languages
{
    using Extensions;
    using Newtonsoft.Json;
    using Util;

    internal class RiakPhaseLanguageJavascript : IRiakPhaseLanguage
    {
        private string name;
        private string source;
        private string bucket;
        private string key;

        public void Name(string name)
        {
            this.name = name;
        }

        public void Source(string source)
        {
            this.source = source;
        }

        public void BucketKey(string bucket, string key)
        {
            this.bucket = bucket;
            this.key = key;
        }

        public void WriteJson(JsonWriter writer)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                System.Diagnostics.Debug.Assert(string.IsNullOrWhiteSpace(bucket), "Bucket should be empty if Name specified");
                System.Diagnostics.Debug.Assert(string.IsNullOrWhiteSpace(key), "Key should be empty if Name specified");
                System.Diagnostics.Debug.Assert(string.IsNullOrWhiteSpace(source), "Source should be empty if Name specified");
            }
            else if (!string.IsNullOrWhiteSpace(source))
            {
                System.Diagnostics.Debug.Assert(string.IsNullOrWhiteSpace(bucket), "Bucket should be empty if Name specified");
                System.Diagnostics.Debug.Assert(string.IsNullOrWhiteSpace(key), "Key should be empty if Name specified");
                System.Diagnostics.Debug.Assert(string.IsNullOrWhiteSpace(name), "Name should be empty if Name specified");
            }
            else
            {
                System.Diagnostics.Debug.Assert(!string.IsNullOrWhiteSpace(bucket), "Bucket should not be empty");
                System.Diagnostics.Debug.Assert(!string.IsNullOrWhiteSpace(key), "Key should not be empty");
            }

            writer.WriteSpecifiedProperty("language", RiakConstants.MapReduceLanguage.JavaScript)
                .WriteSpecifiedProperty("source", source)
                .WriteSpecifiedProperty("name", name)
                .WriteSpecifiedProperty("bucket", bucket)
                .WriteSpecifiedProperty("key", key);
        }
    }
}
