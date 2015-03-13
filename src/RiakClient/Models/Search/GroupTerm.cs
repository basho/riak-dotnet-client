// <copyright file="GroupTerm.cs" company="Basho Technologies, Inc.">
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
    /// <summary>
    /// Represents a Lucene grouped search term.
    /// </summary>
    public class GroupTerm : Term
    {
        private readonly Term term;

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupTerm"/> class.
        /// </summary>
        /// <param name="search">The fluent search to add this term to.</param>
        /// <param name="field">The field to search.</param>
        /// <param name="term">The <see cref="Term"/> group to search the <paramref name="field"/> for.</param>
        internal GroupTerm(RiakFluentSearch search, string field, Term term)
            : base(search, field)
        {
            this.term = term;
        }

        /// <summary>
        /// Returns the term in a Lucene query string format.
        /// </summary>
        /// <returns>
        /// A string that represents the query term.</returns>
        public override string ToString()
        {
            Term tmpTerm = term;

            while (tmpTerm.Owner != null)
            {
                tmpTerm = tmpTerm.Owner;
            }

            return Prefix() + "(" + tmpTerm + ")" + Suffix();
        }
    }
}
