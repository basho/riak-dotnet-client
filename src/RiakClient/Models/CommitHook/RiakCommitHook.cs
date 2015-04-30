// <copyright file="RiakCommitHook.cs" company="Basho Technologies, Inc.">
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

namespace RiakClient.Models.CommitHook
{
    using System;
    using System.IO;
    using System.Text;
    using Messages;
    using Newtonsoft.Json;

    /// <summary>
    /// Represents an abstract implementation for Riak commit hooks.
    /// </summary>
    public abstract class RiakCommitHook : IRiakCommitHook, IEquatable<RiakCommitHook>
    {
        /// <inheritdoc/>
        public string ToJsonString()
        {
            /*
             * NB: JsonTextWriter is guaranteed to close the StringWriter
             * https://github.com/JamesNK/Newtonsoft.Json/blob/master/Src/Newtonsoft.Json/JsonTextWriter.cs#L150-L160
             */
            var sb = new StringBuilder();
            var sw = new StringWriter(sb);
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                WriteJson(writer);
            }

            return sb.ToString();
        }

        /// <inheritdoc/>
        public abstract void WriteJson(JsonWriter writer);

        /// <inheritdoc/>
        public abstract RpbCommitHook ToRpbCommitHook();

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="other">The object to compare with the current object.</param>
        /// <returns><b>true</b> if the specified object is equal to the current object, otherwise, <b>false</b>.</returns>
        public abstract bool Equals(RiakCommitHook other);
    }
}
