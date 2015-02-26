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

        protected RiakFluentSearch Search
        {
            get { return search; }
        }

        public RiakFluentSearch Build()
        {
            return search;
        }

        public Term Boost(double boost)
        {
            this.boost = boost;
            return this;
        }

        public Term Not()
        {
            not = true;
            return this;
        }

        public BinaryTerm Or(string value)
        {
            return Or(field, Token.Is(value));
        }

        public BinaryTerm Or(Token value)
        {
            return Or(field, value);
        }

        public BinaryTerm Or(string field, string value)
        {
            return new BinaryTerm(search, field, BinaryTerm.Op.Or, this, Token.Is(value));
        }

        public BinaryTerm Or(string field, Token value)
        {
            return new BinaryTerm(search, field, BinaryTerm.Op.Or, this, value);
        }

        public BinaryTerm OrBetween(string from, string to, bool inclusive = true)
        {
            return OrBetween(field, Token.Is(from), Token.Is(to), inclusive);
        }

        public BinaryTerm OrBetween(string from, Token to, bool inclusive = true)
        {
            return OrBetween(field, Token.Is(from), to, inclusive);
        }

        public BinaryTerm OrBetween(Token from, string to, bool inclusive = true)
        {
            return OrBetween(field, from, Token.Is(to), inclusive);
        }

        public BinaryTerm OrBetween(Token from, Token to, bool inclusive = true)
        {
            return OrBetween(field, from, to, inclusive);
        }

        public BinaryTerm OrBetween(string field, string from, string to, bool inclusive = true)
        {
            return OrBetween(field, Token.Is(from), Token.Is(to), inclusive);
        }

        public BinaryTerm OrBetween(string field, string from, Token to, bool inclusive = true)
        {
            return OrBetween(field, Token.Is(from), to, inclusive);
        }

        public BinaryTerm OrBetween(string field, Token from, string to, bool inclusive = true)
        {
            return OrBetween(field, from, Token.Is(to), inclusive);
        }

        public BinaryTerm OrBetween(string field, Token from, Token to, bool inclusive = true)
        {
            var range = new RangeTerm(search, field, from, to, inclusive);
            return new BinaryTerm(search, field, BinaryTerm.Op.Or, this, range);
        }

        public BinaryTerm Or(string value, Func<Term, Term> groupSetup)
        {
            return Or(field, Token.Is(value), groupSetup);
        }

        public BinaryTerm Or(Token value, Func<Term, Term> groupSetup)
        {
            return Or(field, value, groupSetup);
        }

        public BinaryTerm Or(string field, string value, Func<Term, Term> groupSetup)
        {
            return Or(field, Token.Is(value), groupSetup);
        }

        public BinaryTerm Or(string field, Token value, Func<Term, Term> groupSetup)
        {
            var groupedTerm = groupSetup(new UnaryTerm(search, field, value));
            var groupTerm = new GroupTerm(search, field, groupedTerm);
            return new BinaryTerm(search, field, BinaryTerm.Op.Or, this, groupTerm);
        }

        public BinaryTerm And(string value)
        {
            return And(field, Token.Is(value));
        }

        public BinaryTerm And(Token value)
        {
            return And(field, value);
        }

        public BinaryTerm And(string field, string value)
        {
            return And(field, Token.Is(value));
        }

        public BinaryTerm And(string field, Token value)
        {
            return new BinaryTerm(search, field, BinaryTerm.Op.And, this, value);
        }

        public BinaryTerm AndBetween(string from, string to, bool inclusive = true)
        {
            return AndBetween(field, from, to, inclusive);
        }

        public BinaryTerm AndBetween(string from, Token to, bool inclusive = true)
        {
            return AndBetween(field, Token.Is(from), to, inclusive);
        }

        public BinaryTerm AndBetween(Token from, string to, bool inclusive = true)
        {
            return AndBetween(field, from, Token.Is(to), inclusive);
        }

        public BinaryTerm AndBetween(Token from, Token to, bool inclusive = true)
        {
            return AndBetween(field, from, to, inclusive);
        }

        public BinaryTerm AndBetween(string field, string from, string to, bool inclusive = true)
        {
            return AndBetween(field, Token.Is(from), Token.Is(to), inclusive);
        }

        public BinaryTerm AndBetween(string field, string from, Token to, bool inclusive = true)
        {
            return AndBetween(field, Token.Is(from), to, inclusive);
        }

        public BinaryTerm AndBetween(string field, Token from, string to, bool inclusive = true)
        {
            return AndBetween(field, from, Token.Is(to), inclusive);
        }

        public BinaryTerm AndBetween(string field, Token from, Token to, bool inclusive = true)
        {
            var range = new RangeTerm(search, field, from, to, inclusive);
            return new BinaryTerm(search, field, BinaryTerm.Op.And, this, range);
        }

        public BinaryTerm And(string value, Func<Term, Term> groupSetup)
        {
            return And(Token.Is(value), groupSetup);
        }

        public BinaryTerm And(Token value, Func<Term, Term> groupSetup)
        {
            return And(field, value, groupSetup);
        }

        public BinaryTerm And(string field, string value, Func<Term, Term> groupSetup)
        {
            return And(field, Token.Is(value), groupSetup);
        }

        public BinaryTerm And(string field, Token value, Func<Term, Term> groupSetup)
        {
            var groupedTerm = groupSetup(new UnaryTerm(search, field, value));
            var groupTerm = new GroupTerm(search, field, groupedTerm);
            return new BinaryTerm(search, field, BinaryTerm.Op.And, this, groupTerm);
        }

        public Term AndProximity(double proximity, params string[] words)
        {
            return AndProximity(field, proximity, words);
        }

        public Term AndProximity(string field, double proximity, params string[] words)
        {
            var term = new ProximityTerm(search, field, proximity, words);
            return new BinaryTerm(search, field, BinaryTerm.Op.And, this, term);
        }

        public Term OrProximity(double proximity, params string[] words)
        {
            return OrProximity(field, proximity, words);
        }

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
