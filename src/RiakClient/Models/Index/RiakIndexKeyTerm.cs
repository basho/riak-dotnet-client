// <copyright file="RiakIndexKeyTerm.cs" company="Basho Technologies, Inc.">
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

namespace RiakClient.Models.Index
{
    /// <summary>
    /// Represents an index result Key-Term pair.
    /// </summary>
    public class RiakIndexKeyTerm
    {
        private readonly string key;
        private readonly string term;

        /// <summary>
        /// Initializes a new instance of the <see cref="RiakIndexKeyTerm" /> class.
        /// </summary>
        /// <param name="key">The result key to use.</param>
        public RiakIndexKeyTerm(string key)
        {
            this.key = key;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RiakIndexKeyTerm" /> class.
        /// </summary>
        /// <param name="key">The result key to use.</param>
        /// <param name="term">The matchint term to use.</param>
        public RiakIndexKeyTerm(string key, string term)
            : this(key)
        {
            this.term = term;
        }

        /// <summary>
        /// The result key.
        /// </summary>
        public string Key
        {
            get { return key; }
        }

        /// <summary>
        /// The matching term.
        /// </summary>
        public string Term
        {
            get { return term; }
        }
    }
}
