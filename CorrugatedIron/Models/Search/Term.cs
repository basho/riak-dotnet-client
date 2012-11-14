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
        private static readonly Regex EncodeRegex = new Regex(@"(["" \\'\(\)\[\]\\:\+\-\/\?])");

        private double? _boost;
        private double? _proximity;
        private bool _not;

        protected RiakFluentSearch Search;

        internal Term Owner { get; set; }

        protected Term(RiakFluentSearch search)
        {
            Search = search;
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

        public Term Not()
        {
            _not = true;
            return this;
        }

        public BinaryTerm Or(string value)
        {
            return new BinaryTerm(Search, BinaryTerm.Op.Or, this, value);
        }

        public BinaryTerm OrRange(string from, string to, bool inclusive = false)
        {
            var range = new RangeTerm(Search, from, to, inclusive);
            return new BinaryTerm(Search, BinaryTerm.Op.Or, this, range);
        }

        public BinaryTerm Or(string value, Func<Term, Term> groupSetup)
        {
            var groupedTerm = groupSetup(new UnaryTerm(Search, value));
            var groupTerm = new GroupTerm(Search, groupedTerm);
            return new BinaryTerm(Search, BinaryTerm.Op.Or, this, groupTerm);
        }

        public BinaryTerm And(string value)
        {
            return new BinaryTerm(Search, BinaryTerm.Op.And, this, value);
        }

        public BinaryTerm AndRange(string from, string to, bool inclusive = false)
        {
            var range = new RangeTerm(Search, from, to, inclusive);
            return new BinaryTerm(Search, BinaryTerm.Op.And, this, range);
        }

        public BinaryTerm And(string value, Func<Term, Term> groupSetup)
        {
            var groupedTerm = groupSetup(new UnaryTerm(Search, value));
            var groupTerm = new GroupTerm(Search, groupedTerm);
            return new BinaryTerm(Search, BinaryTerm.Op.And, this, groupTerm);
        }

        protected static string Encode(string value)
        {
            return value != null ? EncodeRegex.Replace(value, m => "\\" + m.Value) : string.Empty;
        }
    }
}