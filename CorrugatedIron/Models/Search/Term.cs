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

using System;
using System.Text.RegularExpressions;

namespace CorrugatedIron.Models.Search
{
    public abstract class Term
    {
        private readonly string _field;

        private double? _boost;
        private double? _proximity;
        private bool _not;

        protected RiakFluentSearch Search;

        internal Term Owner { get; set; }

        protected Term(RiakFluentSearch search, string field)
        {
            Search = search;
            _field = field;
        }

        public RiakFluentSearch Build()
        {
            return Search;
        }

        public Term Boost(double boost)
        {
            _boost = boost;
            return this;
        }

        public Term Proximity(double proximity)
        {
            _proximity = proximity;
            return this;
        }

        internal string Suffix()
        {
            return (_boost.HasValue ? "^" + _boost.Value : string.Empty) +
                (_proximity.HasValue ?  "~" + _proximity.Value : string.Empty);
        }

        internal string Prefix()
        {
            return _not ? "NOT " : string.Empty;
        }

        internal string Field()
        {
            return string.IsNullOrWhiteSpace(_field) ? string.Empty : _field + ":";
        }

        public Term Not()
        {
            _not = true;
            return this;
        }

        public BinaryTerm Or(string value)
        {
            return Or(_field, Token.Is(value));
        }

        public BinaryTerm Or(Token value)
        {
            return Or(_field, value);
        }

        public BinaryTerm Or(string field, string value)
        {
            return new BinaryTerm(Search, field, BinaryTerm.Op.Or, this, Token.Is(value));
        }

        public BinaryTerm Or(string field, Token value)
        {
            return new BinaryTerm(Search, field, BinaryTerm.Op.Or, this, value);
        }

        public BinaryTerm OrRange(string from, string to, bool inclusive = false)
        {
            return OrRange(_field, Token.Is(from), Token.Is(to), inclusive);
        }

        public BinaryTerm OrRange(string from, Token to, bool inclusive = false)
        {
            return OrRange(_field, Token.Is(from), to, inclusive);
        }

        public BinaryTerm OrRange(Token from, string to, bool inclusive = false)
        {
            return OrRange(_field, from, Token.Is(to), inclusive);
        }

        public BinaryTerm OrRange(Token from, Token to, bool inclusive = false)
        {
            return OrRange(_field, from, to, inclusive);
        }

        public BinaryTerm OrRange(string field, string from, string to, bool inclusive = false)
        {
            return OrRange(field, Token.Is(from), Token.Is(to), inclusive);
        }

        public BinaryTerm OrRange(string field, string from, Token to, bool inclusive = false)
        {
            return OrRange(field, Token.Is(from), to, inclusive);
        }

        public BinaryTerm OrRange(string field, Token from, string to, bool inclusive = false)
        {
            return OrRange(field, from, Token.Is(to), inclusive);
        }

        public BinaryTerm OrRange(string field, Token from, Token to, bool inclusive = false)
        {
            var range = new RangeTerm(Search, field, from, to, inclusive);
            return new BinaryTerm(Search, field, BinaryTerm.Op.Or, this, range);
        }

        public BinaryTerm Or(string value, Func<Term, Term> groupSetup)
        {
            return Or(_field, Token.Is(value), groupSetup);
        }

        public BinaryTerm Or(Token value, Func<Term, Term> groupSetup)
        {
            return Or(_field, value, groupSetup);
        }

        public BinaryTerm Or(string field, string value, Func<Term, Term> groupSetup)
        {
            return Or(field, Token.Is(value), groupSetup);
        }

        public BinaryTerm Or(string field, Token value, Func<Term, Term> groupSetup)
        {
            var groupedTerm = groupSetup(new UnaryTerm(Search, field, value));
            var groupTerm = new GroupTerm(Search, field, groupedTerm);
            return new BinaryTerm(Search, field, BinaryTerm.Op.Or, this, groupTerm);
        }

        public BinaryTerm And(string value)
        {
            return And(_field, Token.Is(value));
        }

        public BinaryTerm And(Token value)
        {
            return And(_field, value);
        }

        public BinaryTerm And(string field, string value)
        {
            return And(field, Token.Is(value));
        }

        public BinaryTerm And(string field, Token value)
        {
            return new BinaryTerm(Search, field, BinaryTerm.Op.And, this, value);
        }

        public BinaryTerm AndRange(string from, string to, bool inclusive = false)
        {
            return AndRange(_field, from, to, inclusive);
        }

        public BinaryTerm AndRange(string from, Token to, bool inclusive = false)
        {
            return AndRange(_field, Token.Is(from), to, inclusive);
        }

        public BinaryTerm AndRange(Token from, string to, bool inclusive = false)
        {
            return AndRange(_field, from, Token.Is(to), inclusive);
        }

        public BinaryTerm AndRange(Token from, Token to, bool inclusive = false)
        {
            return AndRange(_field, from, to, inclusive);
        }

        public BinaryTerm AndRange(string field, string from, string to, bool inclusive = false)
        {
            return AndRange(field, Token.Is(from), Token.Is(to), inclusive);
        }

        public BinaryTerm AndRange(string field, string from, Token to, bool inclusive = false)
        {
            return AndRange(field, Token.Is(from), to, inclusive);
        }

        public BinaryTerm AndRange(string field, Token from, string to, bool inclusive = false)
        {
            return AndRange(field, from, Token.Is(to), inclusive);
        }

        public BinaryTerm AndRange(string field, Token from, Token to, bool inclusive = false)
        {
            var range = new RangeTerm(Search, field, from, to, inclusive);
            return new BinaryTerm(Search, field, BinaryTerm.Op.And, this, range);
        }

        public BinaryTerm And(string value, Func<Term, Term> groupSetup)
        {
            return And(Token.Is(value), groupSetup);
        }

        public BinaryTerm And(Token value, Func<Term, Term> groupSetup)
        {
            return And(_field, value, groupSetup);
        }

        public BinaryTerm And(string field, string value, Func<Term, Term> groupSetup)
        {
            return And(field, Token.Is(value), groupSetup);
        }

        public BinaryTerm And(string field, Token value, Func<Term, Term> groupSetup)
        {
            var groupedTerm = groupSetup(new UnaryTerm(Search, field, value));
            var groupTerm = new GroupTerm(Search, field, groupedTerm);
            return new BinaryTerm(Search, field, BinaryTerm.Op.And, this, groupTerm);
        }
    }
}