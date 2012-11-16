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
        private string _field;
        private Term _term;
        private bool _grouped;

        public RiakFluentSearch(string bucket, string field)
        {
            _bucket = bucket;
            _field = field;
        }

        public Term Search(string value)
        {
            _term = new UnaryTerm(this, _field, value);
            return _term;
        }

        public Term Group(string value, Func<Term, Term> groupSetup)
        {
            var groupedTerm = groupSetup(new UnaryTerm(this, _field, value));
            _grouped = true;
            _term = new GroupTerm(this, _field, groupedTerm);
            return _term;
        }

        public Term Search(string from, string to, bool inclusive = false)
        {
            _term = new RangeTerm(this, _field, from, to, inclusive);
            return _term;
        }

        public override string ToString()
        {
            var term = _term;
            while (term.Owner != null)
            {
                term = term.Owner;
            }

            return _bucket + "." + (_grouped ? _field + ":" : string.Empty) + term;
        }
    }
}
