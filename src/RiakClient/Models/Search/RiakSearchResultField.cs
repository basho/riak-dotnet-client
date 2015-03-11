// <copyright file="RiakSearchResultField.cs" company="Basho Technologies, Inc.">
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
    using Extensions;
    using Messages;

    /// <summary>
    /// Represents a single Riak Search result field.
    /// </summary>
    public class RiakSearchResultField
    {
        private readonly string key;
        private readonly string value;

        internal RiakSearchResultField(RpbPair field)
        {
            this.key = field.key.FromRiakString();
            this.value = field.value.FromRiakString();
        }

        /// <summary>
        /// The key of the result field.
        /// </summary>
        public string Key
        {
            get { return key; }
        }

        /// <summary>
        /// The value of the result field.
        /// </summary>
        public string Value
        {
            get { return value; }
        }
    }
}
