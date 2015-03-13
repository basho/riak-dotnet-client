// <copyright file="Term.cs" company="Basho Technologies, Inc.">
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
    /// Represents an abstract Lucene search term.
    /// </summary>
    public abstract class Term
    {
        private readonly string field;
        private readonly RiakFluentSearch search;
        private double? boost;
        private bool not;

        protected Term(RiakFluentSearch search, string field)
        {
            this.search = search;
            this.field = field;
        }

        internal Term Owner { get; set; }

        /// <summary>
        /// Returns a new <see cref="RiakFluentSearch"/> object based on this Term.
        /// </summary>
        /// <returns>A configured <see cref="RiakFluentSearch"/> object.</returns>
        protected RiakFluentSearch Search
        {
            get { return search; }
        }

        /// <summary>
        /// Builds the Term into a new <see cref="RiakFluentSearch"/> object.
        /// </summary>
        /// <returns>A configured <see cref="RiakFluentSearch"/> object.</returns>
        public RiakFluentSearch Build()
        {
            return search;
        }

        /// <summary>
        /// Boosts the relevance level of the this Term when matches are found.
        /// </summary>
        /// <param name="boost">The amount to set this Term's boost value to.</param>
        /// <returns>A boosted <see cref="Term"/>.</returns>
        /// <remarks>The default boost value is 1.0.</remarks>
        public Term Boost(double boost)
        {
            this.boost = boost;
            return this;
        }

        /// <summary>
        /// Inverts any matches so that the documents found by this Term are excluded from the result set.
        /// </summary>
        /// <returns>An inverted Term.</returns>
        public Term Not()
        {
            not = true;
            return this;
        }

        /// <summary>
        /// Combines this Term, using a logical OR, with a new one 
        /// searching this Term's field for the provided <paramref name="value"/> string.
        /// </summary>
        /// <param name="value">The other value to match.</param>
        /// <returns>
        /// A new <see cref="BinaryTerm"/> based on this Term and the provided <paramref name="value"/>.
        /// </returns>
        public BinaryTerm Or(string value)
        {
            return Or(field, Token.Is(value));
        }
        
        /// <summary>
        /// Combines this Term, using a logical OR, with a new one searching 
        /// this Term's field for the provided <paramref name="value"/> Token.
        /// </summary>
        /// <param name="value">The other value to match.</param>
        /// <returns>
        /// A new <see cref="BinaryTerm"/> based on this Term and the provided <paramref name="value"/>.
        /// </returns>
        public BinaryTerm Or(Token value)
        {
            return Or(field, value);
        }

        /// <summary>
        /// Combines this Term, using a logical OR, with a new one searching the <paramref name="field"/> 
        /// field for <paramref name="value"/> string.
        /// </summary>
        /// <param name="field">The other field to search.</param>
        /// <param name="value">The other value to match.</param>
        /// <returns>
        /// A new <see cref="BinaryTerm"/> based on this Term and the provided <paramref name="value"/>.
        /// </returns>
        public BinaryTerm Or(string field, string value)
        {
            return new BinaryTerm(search, field, BinaryTerm.Op.Or, this, Token.Is(value));
        }

        /// <summary>
        /// Combines this Term, using a logical OR, with a new one searching 
        /// the <paramref name="field"/> field for the <paramref name="value"/> Token.
        /// </summary>
        /// <param name="field">The other field to search.</param>
        /// <param name="value">The other value to match.</param>
        /// <returns>
        /// A new <see cref="BinaryTerm"/> based on this Term and the provided <paramref name="value"/>.
        /// </returns>
        public BinaryTerm Or(string field, Token value)
        {
            return new BinaryTerm(search, field, BinaryTerm.Op.Or, this, value);
        }

        /// <summary>
        /// Combines this Term, using a logical OR, with a new Range Term
        /// searching the current field for values between <paramref name="from"/>
        /// and <paramref name="to"/>.
        /// </summary>
        /// <param name="from">The lower bound of values to search for.</param>
        /// <param name="to">The upper bound of values to search for.</param>
        /// <param name="inclusive">The option to include the bounds in the range or not.</param>
        /// <returns>
        /// A new <see cref="BinaryTerm"/> based on this Term and the provided parameters.
        /// </returns>
        public BinaryTerm OrBetween(string from, string to, bool inclusive = true)
        {
            return OrBetween(field, Token.Is(from), Token.Is(to), inclusive);
        }

        /// <summary>
        /// Combine this Term, using a logical OR, with a new Range Term
        /// searching the current field for values between <paramref name="from"/>
        /// and <paramref name="to"/>.
        /// </summary>
        /// <param name="from">The lower bound of values to search for.</param>
        /// <param name="to">The upper bound of values to search for.</param>
        /// <param name="inclusive">The option to include the bounds in the range or not.</param>
        /// <returns>
        /// A new <see cref="BinaryTerm"/> based on this Term and the provided parameters.
        /// </returns>
        public BinaryTerm OrBetween(string from, Token to, bool inclusive = true)
        {
            return OrBetween(field, Token.Is(from), to, inclusive);
        }

        /// <summary>
        /// Combine this Term, using a logical OR, with a new Range Term
        /// searching the current field for values between <paramref name="from"/>
        /// and <paramref name="to"/>.
        /// </summary>
        /// <param name="from">The lower bound of values to search for.</param>
        /// <param name="to">The upper bound of values to search for.</param>
        /// <param name="inclusive">The option to include the bounds in the range or not.</param>
        /// <returns>
        /// A new <see cref="BinaryTerm"/> based on this Term and the provided parameters.
        /// </returns>
        public BinaryTerm OrBetween(Token from, string to, bool inclusive = true)
        {
            return OrBetween(field, from, Token.Is(to), inclusive);
        }

        /// <summary>
        /// Combine this Term, using a logical Or, with a new Range Term
        /// searching the current field for values between <paramref name="from"/>
        /// and <paramref name="to"/>.
        /// </summary>
        /// <param name="from">The lower bound of values to search for.</param>
        /// <param name="to">The upper bound of values to search for.</param>
        /// <param name="inclusive">The option to include the bounds in the range or not.</param>
        /// <returns>
        /// A new <see cref="BinaryTerm"/> based on this Term and the provided parameters.
        /// </returns>
        public BinaryTerm OrBetween(Token from, Token to, bool inclusive = true)
        {
            return OrBetween(field, from, to, inclusive);
        }

        /// <summary>
        /// Combine this Term, using a logical OR, with a new Range Term
        /// searching <paramref name="field"/> for values between <paramref name="from"/>
        /// and <paramref name="to"/>.
        /// </summary>
        /// <param name="field">The other field to search.</param>
        /// <param name="from">The lower bound of values to search for.</param>
        /// <param name="to">The upper bound of values to search for.</param>
        /// <param name="inclusive">The option to include the bounds in the range or not.</param>
        /// <returns>
        /// A new <see cref="BinaryTerm"/> based on this Term and the provided parameters.
        /// </returns>
        public BinaryTerm OrBetween(string field, string from, string to, bool inclusive = true)
        {
            return OrBetween(field, Token.Is(from), Token.Is(to), inclusive);
        }

        /// <summary>
        /// Combine this Term, using a logical OR, with a new Range Term
        /// searching <paramref name="field"/> for values between <paramref name="from"/>
        /// and <paramref name="to"/>.
        /// </summary>
        /// <param name="field">The other field to search.</param>
        /// <param name="from">The lower bound of values to search for.</param>
        /// <param name="to">The upper bound of values to search for.</param>
        /// <param name="inclusive">The option to include the bounds in the range or not.</param>
        /// <returns>
        /// A new <see cref="BinaryTerm"/> based on this Term and the provided parameters.
        /// </returns>
        public BinaryTerm OrBetween(string field, string from, Token to, bool inclusive = true)
        {
            return OrBetween(field, Token.Is(from), to, inclusive);
        }

        /// <summary>
        /// Combine this Term, using a logical OR, with a new Range Term
        /// searching <paramref name="field"/> for values between <paramref name="from"/>
        /// and <paramref name="to"/>.
        /// </summary>
        /// <param name="field">The other field to search.</param>
        /// <param name="from">The lower bound of values to search for.</param>
        /// <param name="to">The upper bound of values to search for.</param>
        /// <param name="inclusive">The option to include the bounds in the range or not.</param>
        /// <returns>
        /// A new <see cref="BinaryTerm"/> based on this Term and the provided parameters.
        /// </returns>
        public BinaryTerm OrBetween(string field, Token from, string to, bool inclusive = true)
        {
            return OrBetween(field, from, Token.Is(to), inclusive);
        }

        /// <summary>
        /// Combine this Term, using a logical OR, with a new Range Term
        /// searching <paramref name="field"/> for values between <paramref name="from"/>
        /// and <paramref name="to"/>.
        /// </summary>
        /// <param name="field">The other field to search.</param>
        /// <param name="from">The lower bound of values to search for.</param>
        /// <param name="to">The upper bound of values to search for.</param>
        /// <param name="inclusive">The option to include the bounds in the range or not.</param>
        /// <returns>
        /// A new <see cref="BinaryTerm"/> based on this Term and the provided parameters.
        /// </returns>
        public BinaryTerm OrBetween(string field, Token from, Token to, bool inclusive = true)
        {
            var range = new RangeTerm(search, field, from, to, inclusive);
            return new BinaryTerm(search, field, BinaryTerm.Op.Or, this, range);
        }

        /// <summary>
        /// Combines a Term searching this field for the <paramref name="value"/> string, using a logical OR, 
        /// with another Term generated by the <paramref name="groupSetup"/> Func.
        /// </summary>
        /// <param name="value">The value to match using the current field.</param>
        /// <param name="groupSetup">
        /// A <see cref="Func{T1,T2}"/> that accepts a <see cref="Term"/> for fluent configuration,
        /// and returns a configured <see cref="Term"/>. 
        /// </param>
        /// <returns>
        /// A new <see cref="BinaryTerm"/> based on the two created <see cref="Term"/>s.
        /// </returns>
        public BinaryTerm Or(string value, Func<Term, Term> groupSetup)
        {
            return Or(field, Token.Is(value), groupSetup);
        }

        /// <summary>
        /// Combines a Term searching this field for the <paramref name="value"/> Token, using a logical OR, 
        /// with another Term generated by the <paramref name="groupSetup"/> Func.
        /// </summary>
        /// <param name="value">The value to match using the current field.</param>
        /// <param name="groupSetup">
        /// A <see cref="Func{T1,T2}"/> that accepts a <see cref="Term"/> for fluent configuration,
        /// and returns a configured <see cref="Term"/>. 
        /// </param>
        /// <returns>
        /// A new <see cref="BinaryTerm"/> based on the two created <see cref="Term"/>s.
        /// </returns>
        public BinaryTerm Or(Token value, Func<Term, Term> groupSetup)
        {
            return Or(field, value, groupSetup);
        }

        /// <summary>
        /// Combines a Term searching the <paramref name="field"/> field for the <paramref name="value"/> string, 
        /// using a logical OR, with another Term generated by the <paramref name="groupSetup"/> Func.
        /// </summary>
        /// <param name="field">The other field to search.</param>
        /// <param name="value">The other value to match.</param>
        /// <param name="groupSetup">
        /// A <see cref="Func{T1,T2}"/> that accepts a <see cref="Term"/> for fluent configuration,
        /// and returns a configured <see cref="Term"/>. 
        /// </param>
        /// <returns>
        /// A new <see cref="BinaryTerm"/> based on the two created <see cref="Term"/>s.
        /// </returns>
        public BinaryTerm Or(string field, string value, Func<Term, Term> groupSetup)
        {
            return Or(field, Token.Is(value), groupSetup);
        }

        /// <summary>
        /// Combines a Term searching the <paramref name="field"/> field for the <paramref name="value"/> Token, 
        /// using a logical OR, with another Term generated by the <paramref name="groupSetup"/> Func.
        /// </summary>
        /// <param name="field">The other field to search.</param>
        /// <param name="value">The other value to match.</param>
        /// <param name="groupSetup">
        /// A <see cref="Func{T1,T2}"/> that accepts a <see cref="Term"/> for fluent configuration,
        /// and returns a configured <see cref="Term"/>. 
        /// </param>
        /// <returns>
        /// A new <see cref="BinaryTerm"/> based on the two created <see cref="Term"/>s.
        /// </returns>
        public BinaryTerm Or(string field, Token value, Func<Term, Term> groupSetup)
        {
            var groupedTerm = groupSetup(new UnaryTerm(search, field, value));
            var groupTerm = new GroupTerm(search, field, groupedTerm);
            return new BinaryTerm(search, field, BinaryTerm.Op.Or, this, groupTerm);
        }

        /// <summary>
        /// Combines this Term, using a logical AND, with a new one searching this 
        /// Term's field for the provided <paramref name="value"/> string.
        /// </summary>
        /// <param name="value">The other value to match.</param>
        /// <returns>
        /// A new <see cref="BinaryTerm"/> based on this Term and the provided <paramref name="value"/>.
        /// </returns>
        public BinaryTerm And(string value)
        {
            return And(field, Token.Is(value));
        }

        /// <summary>
        /// Combines this Term, using a logical AND, with a new one searching this 
        /// Term's field for the provided <paramref name="value"/> Token.
        /// </summary>
        /// <param name="value">The other value to match.</param>
        /// <returns>
        /// A new <see cref="BinaryTerm"/> based on this Term and the provided <paramref name="value"/>.
        /// </returns>
        public BinaryTerm And(Token value)
        {
            return And(field, value);
        }

        /// <summary>
        /// Combines this Term, using a logical AND, with a new one searching the 
        /// <paramref name="field"/> field for the provided <paramref name="value"/> string.
        /// </summary>
        /// <param name="field">The other field to search.</param>
        /// <param name="value">The other value to match.</param>
        /// <returns>
        /// A new <see cref="BinaryTerm"/> based on this Term and the provided <paramref name="value"/>.
        /// </returns>
        public BinaryTerm And(string field, string value)
        {
            return And(field, Token.Is(value));
        }

        /// <summary>
        /// Combines this Term, using a logical AND, with a new one searching the 
        /// <paramref name="field"/> field for the provided <paramref name="value"/> Token.
        /// </summary>
        /// <param name="field">The other field to search.</param>
        /// <param name="value">The other value to match.</param>
        /// <returns>
        /// A new <see cref="BinaryTerm"/> based on this Term and the provided <paramref name="value"/>.
        /// </returns>
        public BinaryTerm And(string field, Token value)
        {
            return new BinaryTerm(search, field, BinaryTerm.Op.And, this, value);
        }

        /// <summary>
        /// Combine this Term, using a logical AND, with a new Range Term
        /// searching the current field for values between <paramref name="from"/>
        /// and <paramref name="to"/>.
        /// </summary>
        /// <param name="from">The lower bound of values to search for.</param>
        /// <param name="to">The upper bound of values to search for.</param>
        /// <param name="inclusive">The option to include the bounds in the range or not.</param>
        /// <returns>
        /// A new <see cref="BinaryTerm"/> based on this Term and the provided parameters.
        /// </returns>
        public BinaryTerm AndBetween(string from, string to, bool inclusive = true)
        {
            return AndBetween(field, from, to, inclusive);
        }

        /// <summary>
        /// Combine this Term, using a logical AND, with a new Range Term
        /// searching the current field for values between <paramref name="from"/>
        /// and <paramref name="to"/>.
        /// </summary>
        /// <param name="from">The lower bound of values to search for.</param>
        /// <param name="to">The upper bound of values to search for.</param>
        /// <param name="inclusive">The option to include the bounds in the range or not.</param>
        /// <returns>
        /// A new <see cref="BinaryTerm"/> based on this Term and the provided parameters.
        /// </returns>
        public BinaryTerm AndBetween(string from, Token to, bool inclusive = true)
        {
            return AndBetween(field, Token.Is(from), to, inclusive);
        }

        /// <summary>
        /// Combine this Term, using a logical AND, with a new Range Term
        /// searching the current field for values between <paramref name="from"/>
        /// and <paramref name="to"/>.
        /// </summary>
        /// <param name="from">The lower bound of values to search for.</param>
        /// <param name="to">The upper bound of values to search for.</param>
        /// <param name="inclusive">The option to include the bounds in the range or not.</param>
        /// <returns>
        /// A new <see cref="BinaryTerm"/> based on this Term and the provided parameters.
        /// </returns>
        public BinaryTerm AndBetween(Token from, string to, bool inclusive = true)
        {
            return AndBetween(field, from, Token.Is(to), inclusive);
        }

        /// <summary>
        /// Combine this Term, using a logical AND, with a new Range Term
        /// searching the current field for values between <paramref name="from"/>
        /// and <paramref name="to"/>.
        /// </summary>
        /// <param name="from">The lower bound of values to search for.</param>
        /// <param name="to">The upper bound of values to search for.</param>
        /// <param name="inclusive">The option to include the bounds in the range or not.</param>
        /// <returns>
        /// A new <see cref="BinaryTerm"/> based on this Term and the provided parameters.
        /// </returns>
        public BinaryTerm AndBetween(Token from, Token to, bool inclusive = true)
        {
            return AndBetween(field, from, to, inclusive);
        }

        /// <summary>
        /// Combine this Term, using a logical AND, with a new Range Term
        /// searching <paramref name="field"/> for values between <paramref name="from"/>
        /// and <paramref name="to"/>.
        /// </summary>
        /// <param name="field">The other field to search.</param>
        /// <param name="from">The lower bound of values to search for.</param>
        /// <param name="to">The upper bound of values to search for.</param>
        /// <param name="inclusive">The option to include the bounds in the range or not.</param>
        /// <returns>
        /// A new <see cref="BinaryTerm"/> based on this Term and the provided parameters.
        /// </returns>
        public BinaryTerm AndBetween(string field, string from, string to, bool inclusive = true)
        {
            return AndBetween(field, Token.Is(from), Token.Is(to), inclusive);
        }

        /// <summary>
        /// Combine this Term, using a logical AND, with a new Range Term
        /// searching <paramref name="field"/> for values between <paramref name="from"/>
        /// and <paramref name="to"/>.
        /// </summary>
        /// <param name="field">The other field to search.</param>
        /// <param name="from">The lower bound of values to search for.</param>
        /// <param name="to">The upper bound of values to search for.</param>
        /// <param name="inclusive">The option to include the bounds in the range or not.</param>
        /// <returns>
        /// A new <see cref="BinaryTerm"/> based on this Term and the provided parameters.
        /// </returns>
        public BinaryTerm AndBetween(string field, string from, Token to, bool inclusive = true)
        {
            return AndBetween(field, Token.Is(from), to, inclusive);
        }

        /// <summary>
        /// Combine this Term, using a logical AND, with a new Range Term
        /// searching <paramref name="field"/> for values between <paramref name="from"/>
        /// and <paramref name="to"/>.
        /// </summary>
        /// <param name="field">The other field to search.</param>
        /// <param name="from">The lower bound of values to search for.</param>
        /// <param name="to">The upper bound of values to search for.</param>
        /// <param name="inclusive">The option to include the bounds in the range or not.</param>
        /// <returns>
        /// A new <see cref="BinaryTerm"/> based on this Term and the provided parameters.
        /// </returns>
        public BinaryTerm AndBetween(string field, Token from, string to, bool inclusive = true)
        {
            return AndBetween(field, from, Token.Is(to), inclusive);
        }

        /// <summary>
        /// Combine this Term, using a logical AND, with a new Range Term
        /// searching <paramref name="field"/> for values between <paramref name="from"/>
        /// and <paramref name="to"/>.
        /// </summary>
        /// <param name="field">The other field to search.</param>
        /// <param name="from">The lower bound of values to search for.</param>
        /// <param name="to">The upper bound of values to search for.</param>
        /// <param name="inclusive">The option to include the bounds in the range or not.</param>
        /// <returns>
        /// A new <see cref="BinaryTerm"/> based on this Term and the provided parameters.
        /// </returns>
        public BinaryTerm AndBetween(string field, Token from, Token to, bool inclusive = true)
        {
            var range = new RangeTerm(search, field, from, to, inclusive);
            return new BinaryTerm(search, field, BinaryTerm.Op.And, this, range);
        }

        /// <summary>
        /// Combines a Term searching this field for the <paramref name="value"/> string, using a logical AND, 
        /// with another Term generated by the <paramref name="groupSetup"/> Func.
        /// </summary>
        /// <param name="value">The value to match using the current field.</param>
        /// <param name="groupSetup">
        /// A <see cref="Func{T1,T2}"/> that accepts a <see cref="Term"/> for fluent configuration,
        /// and returns a configured <see cref="Term"/>. 
        /// </param>
        /// <returns>
        /// A new <see cref="BinaryTerm"/> based on the two created <see cref="Term"/>s.
        /// </returns>
        public BinaryTerm And(string value, Func<Term, Term> groupSetup)
        {
            return And(Token.Is(value), groupSetup);
        }

        /// <summary>
        /// Combines a Term searching this field for the <paramref name="value"/> Token, using a logical AND, 
        /// with another Term generated by the <paramref name="groupSetup"/> Func.
        /// </summary>
        /// <param name="value">The value to match using the current field.</param>
        /// <param name="groupSetup">
        /// A <see cref="Func{T1,T2}"/> that accepts a <see cref="Term"/> for fluent configuration,
        /// and returns a configured <see cref="Term"/>. 
        /// </param>
        /// <returns>
        /// A new <see cref="BinaryTerm"/> based on the two created <see cref="Term"/>s.
        /// </returns>
        public BinaryTerm And(Token value, Func<Term, Term> groupSetup)
        {
            return And(field, value, groupSetup);
        }

        /// <summary>
        /// Combines a Term searching the <paramref name="field"/> field for the <paramref name="value"/> string, 
        /// using a logical AND, with another Term generated by the <paramref name="groupSetup"/> Func.
        /// </summary>
        /// <param name="field">The other field to search.</param>
        /// <param name="value">The other value to match.</param>
        /// <param name="groupSetup">
        /// A <see cref="Func{T1,T2}"/> that accepts a <see cref="Term"/> for fluent configuration,
        /// and returns a configured <see cref="Term"/>. 
        /// </param>
        /// <returns>
        /// A new <see cref="BinaryTerm"/> based on the two created <see cref="Term"/>s.
        /// </returns>
        public BinaryTerm And(string field, string value, Func<Term, Term> groupSetup)
        {
            return And(field, Token.Is(value), groupSetup);
        }

        /// <summary>
        /// Combines a Term searching the <paramref name="field"/> field for the <paramref name="value"/> Token, 
        /// using a logical AND, with another Term generated by the <paramref name="groupSetup"/> Func.
        /// </summary>
        /// <param name="field">The other field to search.</param>
        /// <param name="value">The other value to match.</param>
        /// <param name="groupSetup">
        /// A <see cref="Func{T1,T2}"/> that accepts a <see cref="Term"/> for fluent configuration,
        /// and returns a configured <see cref="Term"/>. 
        /// </param>
        /// <returns>
        /// A new <see cref="BinaryTerm"/> based on the two created <see cref="Term"/>s.
        /// </returns>
        public BinaryTerm And(string field, Token value, Func<Term, Term> groupSetup)
        {
            var groupedTerm = groupSetup(new UnaryTerm(search, field, value));
            var groupTerm = new GroupTerm(search, field, groupedTerm);
            return new BinaryTerm(search, field, BinaryTerm.Op.And, this, groupTerm);
        }

        /// <summary>
        /// Combine this Term, using a logical AND, with a new one that searches the current field 
        /// for a set of <paramref name="words"/> that are within a certain distance 
        /// (<paramref name="proximity"/>) of each other.
        /// </summary>
        /// <param name="proximity">The maximum distance the words can be from each other.</param>
        /// <param name="words">The set of words to find within a certain distance of each other.</param>
        /// <returns>A constructed search <see cref="Term"/>.</returns>
        public Term AndProximity(double proximity, params string[] words)
        {
            return AndProximity(field, proximity, words);
        }

        /// <summary>
        /// Combine this Term, using a logical AND, with a new one that searches the <paramref name="field"/> 
        ///  field for a set of <paramref name="words"/> that are within a certain distance 
        /// (<paramref name="proximity"/>) of each other.
        /// </summary>
        /// <param name="field">The other field to search.</param>
        /// <param name="proximity">The maximum distance the words can be from each other.</param>
        /// <param name="words">The set of words to find within a certain distance of each other.</param>
        /// <returns>A constructed search <see cref="Term"/>.</returns>
        public Term AndProximity(string field, double proximity, params string[] words)
        {
            var term = new ProximityTerm(search, field, proximity, words);
            return new BinaryTerm(search, field, BinaryTerm.Op.And, this, term);
        }

        /// <summary>
        /// Combine this Term, using a logical OR, with a new one that searches the current field 
        /// for a set of <paramref name="words"/> that are within a certain distance 
        /// (<paramref name="proximity"/>) of each other.
        /// </summary>
        /// <param name="proximity">The maximum distance the words can be from each other.</param>
        /// <param name="words">The set of words to find within a certain distance of each other.</param>
        /// <returns>A constructed search <see cref="Term"/>.</returns>
        public Term OrProximity(double proximity, params string[] words)
        {
            return OrProximity(field, proximity, words);
        }

        /// <summary>
        /// Combine this Term, using a logical OR, with a new one that searches the <paramref name="field"/> 
        ///  field for a set of <paramref name="words"/> that are within a certain distance 
        /// (<paramref name="proximity"/>) of each other.
        /// </summary>
        /// <param name="field">The other field to search.</param>
        /// <param name="proximity">The maximum distance the words can be from each other.</param>
        /// <param name="words">The set of words to find within a certain distance of each other.</param>
        /// <returns>A constructed search <see cref="Term"/>.</returns>
        public Term OrProximity(string field, double proximity, params string[] words)
        {
            var term = new ProximityTerm(search, field, proximity, words);
            return new BinaryTerm(search, field, BinaryTerm.Op.Or, this, term);
        }

        internal string Suffix()
        {
            return boost.HasValue ? "^" + boost.Value : string.Empty;
        }

        internal string Prefix()
        {
            return not ? "NOT " : string.Empty;
        }

        internal string Field()
        {
            return string.IsNullOrWhiteSpace(field) ? string.Empty : field + ":";
        }
    }
}
