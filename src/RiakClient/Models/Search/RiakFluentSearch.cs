// <copyright file="RiakFluentSearch.cs" company="Basho Technologies, Inc.">
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
    using System;

    /// <summary>
    /// A builder class for constructing Riak Search queries in a fluent manner.
    /// </summary>
    public class RiakFluentSearch
    {
        private readonly string index;
        private readonly string field;
        private Term term;
        private bool grouped;

        /// <summary>
        /// Initializes a new instance of the <see cref="RiakFluentSearch"/> class.
        /// </summary>
        /// <param name="index">The index to run the query against.</param>
        /// <param name="field">The first field to start building a search term for.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> cannot be null, an empty string, or whitespace.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="field"/> cannot be null, an empty string, or whitespace.</exception>
        public RiakFluentSearch(string index, string field)
        {
            if (string.IsNullOrWhiteSpace(index))
            {
                throw new ArgumentOutOfRangeException("index", "index cannot be null, an empty string, or whitespace.");
            }

            if (string.IsNullOrWhiteSpace(field))
            {
                throw new ArgumentOutOfRangeException("field", "field cannot be null, an empty string, or whitespace.");
            }

            this.index = index;
            this.field = field;
        }

        /// <summary>
        /// The bucket name to run the query against.
        /// Used only for Legacy Search.
        /// </summary>
        /// <remarks>The <see cref="Index"/> field replaces this one for Legacy Search queries.</remarks>
        [Obsolete("Bucket is deprecated, please use Index instead.", true)]
        public string Bucket
        {
            get { return index; }
        }

        /// <summary>
        /// The index to run the query against.
        /// </summary>
        /// <remarks>Replaces the <see cref="Bucket"/> field for Legacy Search queries.</remarks>
        public string Index
        {
            get { return index; }
        }

        /// <summary>
        /// Search the current field for an exact match to the <paramref name="value"/> string.
        /// </summary>
        /// <param name="value">The value to search for.</param>
        /// <returns>A constructed search <see cref="Term"/>.</returns>
        public Term Search(string value)
        {
            return Search(Token.Is(value));
        }

        /// <summary>
        /// Search the current field for an exact match to the <paramref name="value"/> token.
        /// </summary>
        /// <param name="value">The token to search for.</param>
        /// <returns>A constructed search <see cref="Term"/>.</returns>
        public Term Search(Token value)
        {
            this.term = new UnaryTerm(this, field, value);
            return this.term;
        }

        /// <summary>
        /// Search the current field for <paramref name="value"/>, 
        /// and group that query with another provided by <paramref name="groupSetup"/>.
        /// </summary>
        /// <param name="value">The field to search values for.</param>
        /// <param name="groupSetup">
        /// An <see cref="Func{T1,T2}"/> that accepts a <see cref="Term"/> for fluent configuration,
        /// and returns that configured <see cref="Term"/>. 
        /// </param>
        /// <returns>A constructed search <see cref="Term"/>.</returns>
        /// <remarks>
        /// Configure the phase with a lambda similar to:
        /// <code>new RiakFluentSearch("bucket", "key").Group("foo", t => t.Or("bar"));</code>
        /// The above filter will return the following grouped query string: 
        /// <code>key:(key:foo OR key:bar)</code>.
        /// </remarks>
        public Term Group(string value, Func<Term, Term> groupSetup)
        {
            return Group(Token.Is(value), groupSetup);
        }

        /// <summary>
        /// Search the current field for the <paramref name="value"/> token,
        /// and group that query with another provided by <paramref name="groupSetup"/>.
        /// </summary>
        /// <param name="value">The token to search values for.</param>
        /// <param name="groupSetup">
        /// A <see cref="Func{T1,T2}"/> that accepts a <see cref="Term"/> for fluent configuration,
        /// and returns that configured <see cref="Term"/>. 
        /// </param>
        /// <returns>A constructed search <see cref="Term"/>.</returns>
        /// <remarks>
        /// Configure the phase with a lambda similar to:
        /// <code>new RiakFluentSearch("bucket", "key").Group("foo", t => t.Or("bar"));</code>
        /// The above filter will return the following grouped query string: 
        /// <code>key:(key:foo OR key:bar)</code>.
        /// </remarks>
        public Term Group(Token value, Func<Term, Term> groupSetup)
        {
            var groupedTerm = groupSetup(new UnaryTerm(this, field, value));
            grouped = true;
            this.term = new GroupTerm(this, field, groupedTerm);
            return this.term;
        }

        /// <summary>
        /// Search the current field for values between <paramref name="from"/> and <paramref name="to"/>.
        /// </summary>
        /// <param name="from">The lower bound of values to search for.</param>
        /// <param name="to">The upper bound of values to search for.</param>
        /// <param name="inclusive">The option to include the bounds in the range or not.</param>
        /// <returns>A constructed search <see cref="Term"/>.</returns>
        public Term Between(string from, string to, bool inclusive = true)
        {
            return Between(Token.Is(from), Token.Is(to), inclusive);
        }

        /// <summary>
        /// Search the current field for values between <paramref name="from"/> and <paramref name="to"/>.
        /// </summary>
        /// <param name="from">The lower bound of values to search for.</param>
        /// <param name="to">The upper bound of values to search for.</param>
        /// <param name="inclusive">The option to include the bounds in the range or not.</param>
        /// <returns>A constructed search <see cref="Term"/>.</returns>
        public Term Between(Token from, string to, bool inclusive = true)
        {
            return Between(from, Token.Is(to), inclusive);
        }

        /// <summary>
        /// Search the current field for values between <paramref name="from"/> and <paramref name="to"/>.
        /// </summary>
        /// <param name="from">The lower bound of values to search for.</param>
        /// <param name="to">The upper bound of values to search for.</param>
        /// <param name="inclusive">The option to include the bounds in the range or not.</param>
        /// <returns>A constructed search <see cref="Term"/>.</returns>
        public Term Between(string from, Token to, bool inclusive = true)
        {
            return Between(Token.Is(from), to, inclusive);
        }

        /// <summary>
        /// Search the current field for values between <paramref name="from"/> and <paramref name="to"/>.
        /// </summary>
        /// <param name="from">The lower bound of values to search for.</param>
        /// <param name="to">The upper bound of values to search for.</param>
        /// <param name="inclusive">The option to include the bounds in the range or not.</param>
        /// <returns>A constructed search <see cref="Term"/>.</returns>
        public Term Between(Token from, Token to, bool inclusive = true)
        {
            this.term = new RangeTerm(this, field, from, to, inclusive);
            return this.term;
        }

        /// <summary>
        /// Search the current field for a set of <paramref name="words"/> that 
        /// are within a certain distance (<paramref name="proximity"/>) of each other.
        /// </summary>
        /// <param name="proximity">The maximum distance the words can be from each other.</param>
        /// <param name="words">The set of words to find within a certain distance of each other.</param>
        /// <returns>A constructed search <see cref="Term"/>.</returns>
        public ProximityTerm Proximity(int proximity, params string[] words)
        {
            return Proximity(field, proximity, words);
        }

        /// <summary>
        /// Search the current field for a set of <paramref name="words"/> that 
        /// are within a certain distance (<paramref name="proximity"/>) of each other.
        /// </summary>
        /// <param name="field">The field to search.</param>
        /// <param name="proximity">The maximum distance the words can be from each other.</param>
        /// <param name="words">The set of words to find within a certain distance of each other.</param>
        /// <returns>A constructed search <see cref="Term"/>.</returns>
        public ProximityTerm Proximity(string field, int proximity, params string[] words)
        {
            var term = new ProximityTerm(this, field, proximity, words);
            this.term = term;
            return term;
        }

        /// <summary>
        /// Returns the search query in a Lucene query string format.
        /// </summary>
        /// <returns>A string that represents the query.</returns>
        public override string ToString()
        {
            Term tmpTerm = term;

            while (tmpTerm.Owner != null)
            {
                tmpTerm = tmpTerm.Owner;
            }

            return (grouped ? field + ":" : string.Empty) + tmpTerm;
        }
    }
}
