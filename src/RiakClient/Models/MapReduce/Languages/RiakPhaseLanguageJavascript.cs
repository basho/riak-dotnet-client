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

using RiakClient.Extensions;
using Newtonsoft.Json;
using RiakClient.Util;

namespace RiakClient.Models.MapReduce.Languages
{
    internal class RiakPhaseLanguageJavascript : IRiakPhaseLanguage
    {
        private string _name;
        private string _source;
        private string _bucket;
        private string _key;

        public void Name(string name)
        {
            _name = name;
        }

        public void Source(string source)
        {
            _source = source;
        }

        public void BucketKey(string bucket, string key)
        {
            _bucket = bucket;
            _key = key;
        }

        public void WriteJson(JsonWriter writer)
        {
            if (!string.IsNullOrWhiteSpace(_name))
            {
                System.Diagnostics.Debug.Assert(string.IsNullOrWhiteSpace(_bucket), "Bucket should be empty if Name specified");
                System.Diagnostics.Debug.Assert(string.IsNullOrWhiteSpace(_key), "Key should be empty if Name specified");
                System.Diagnostics.Debug.Assert(string.IsNullOrWhiteSpace(_source), "Source should be empty if Name specified");
            }
            else if (!string.IsNullOrWhiteSpace(_source))
            {
                System.Diagnostics.Debug.Assert(string.IsNullOrWhiteSpace(_bucket), "Bucket should be empty if Name specified");
                System.Diagnostics.Debug.Assert(string.IsNullOrWhiteSpace(_key), "Key should be empty if Name specified");
                System.Diagnostics.Debug.Assert(string.IsNullOrWhiteSpace(_name), "Name should be empty if Name specified");
            }
            else
            {
                System.Diagnostics.Debug.Assert(!string.IsNullOrWhiteSpace(_bucket), "Bucket should not be empty");
                System.Diagnostics.Debug.Assert(!string.IsNullOrWhiteSpace(_key), "Key should not be empty");
            }

            writer.WriteSpecifiedProperty("language", RiakConstants.MapReduceLanguage.JavaScript)
                .WriteSpecifiedProperty("source", _source)
                .WriteSpecifiedProperty("name", _name)
                .WriteSpecifiedProperty("bucket", _bucket)
                .WriteSpecifiedProperty("key", _key);
        }
    }
}