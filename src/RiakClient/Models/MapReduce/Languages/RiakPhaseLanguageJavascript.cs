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

    internal class RiakPhaseLanguageJavascript : IRiakPhaseLanguage
    {
        private string name;
        private string source;
        private string bucket;
        private string key;

        /// <summary>
        /// Specify a name of the known JavaScript function to execute for this phase.
        /// </summary>
        /// <param name="name">The name of the function to execute.</param>
        public void Name(string name)
        {
            this.name = name;
        }

        /// <summary>
        /// Specify the source code of the JavaScript function to dynamically load and execute for this phase.
        /// </summary>
        /// <param name="source">The source code of the function to execute.</param>
        public void Source(string source)
        {
            this.source = source;
        }

        /// <summary>
        /// Specify a bucket and key where a stored JavaScript function can be dynamically loaded from Riak and executed for this phase
        /// </summary>
        /// <param name="bucket">The bucket name of the JavaScript function's address.</param>
        /// <param name="key">The key of the JavaScript function's address.</param>
        public void BucketKey(string bucket, string key)
        {
            this.bucket = bucket;
            this.key = key;
        }

        /// <inheritdoc/>
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
