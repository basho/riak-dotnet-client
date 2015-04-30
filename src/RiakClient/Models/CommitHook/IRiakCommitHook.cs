// <copyright file="IRiakCommitHook.cs" company="Basho Technologies, Inc.">
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
    using Messages;
    using Newtonsoft.Json;

    /// <summary>
    /// Represents an interface for commit hooks.
    /// </summary>
    public interface IRiakCommitHook
    {
        /// <summary>
        /// Convert the commit hook to a JSON-encoded string.
        /// </summary>
        /// <returns>
        /// A JSON-encoded string.
        /// </returns>
        string ToJsonString();

        /// <summary>
        /// Converts the commit hook to a protobuf message.
        /// </summary>
        /// <returns>
        /// A new instance of a <see cref="RpbCommitHook"/> class.
        /// </returns>
        RpbCommitHook ToRpbCommitHook();

        /// <summary>
        /// Write the commit hook to a <see cref="JsonWriter"/>.
        /// </summary>
        /// <param name="writer">The writer to use.</param>
        void WriteJson(JsonWriter writer);
    }
}
