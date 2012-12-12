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

namespace CorrugatedIron.Models.Search
{
    public class RiakFluentSearch
    {
        private readonly string _bucket;
        private readonly string _field;
        private Term _term;
        private bool _grouped;

        public string Bucket { get { return _bucket; } }

        public RiakFluentSearch(string bucket, string field)
        {
            if (string.IsNullOrWhiteSpace(bucket)) throw new ArgumentNullException("bucket");
            if (string.IsNullOrWhiteSpace(field)) throw new ArgumentNullException("field");

            _bucket = bucket;
            _field = field;
        }

        public Term Search(string value)
        {
            return Search(Token.Is(value));
        }

        public Term Search(Token value)
        {
            _term = new UnaryTerm(this, _field, value);
            return _term;
        }

        public Term Group(string value, Func<Term, Term> groupSetup)
        {
            return Group(Token.Is(value), groupSetup);
        }

        public Term Group(Token value, Func<Term, Term> groupSetup)
        {
            var groupedTerm = groupSetup(new UnaryTerm(this, _field, value));
            _grouped = true;
            _term = new GroupTerm(this, _field, groupedTerm);
            return _term;
        }

        public Term Between(string from, string to, bool inclusive = true)
        {
            return Between(Token.Is(from), Token.Is(to), inclusive);
        }

        public Term Between(Token from, string to, bool inclusive = true)
        {
            return Between(from, Token.Is(to), inclusive);
        }

        public Term Between(string from, Token to, bool inclusive = true)
        {
            return Between(Token.Is(from), to, inclusive);
        }

        public Term Between(Token from, Token to, bool inclusive = true)
        {
            _term = new RangeTerm(this, _field, from, to, inclusive);
            return _term;
        }

        public ProximityTerm Proximity(int proximity, params string[] words)
        {
            return Proximity(_field, proximity, words);
        }

        public ProximityTerm Proximity(string field, int proximity, params string[] words)
        {
            var term = new ProximityTerm(this, field, proximity, words);
            _term = term;
            return term;
        }

        public override string ToString()
        {
            var term = _term;
            while (term.Owner != null)
            {
                term = term.Owner;
            }

            return (_grouped ? _field + ":" : string.Empty) + term;
        }
    }
}
