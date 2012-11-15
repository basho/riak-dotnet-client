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
        private readonly string _index;
        private readonly string _field;
        private Term _term;

        public RiakFluentSearch()
            : this(null, null)
        {
        }

        public RiakFluentSearch(string field)
            : this(null, field)
        {
        }

        public RiakFluentSearch(string index, string field)
        {
            _index = index;
            _field = field;
        }

        public Term Search(string value)
        {
            _term = new UnaryTerm(this, value);
            return _term;
        }

        public Term Group(string value, Func<Term, Term> groupSetup)
        {
            var groupedTerm = groupSetup(new UnaryTerm(this, value));
            _term = new GroupTerm(this, groupedTerm);
            return _term;
        }

        public Term Search(string from, string to, bool inclusive = false)
        {
            _term = new RangeTerm(this, from, to, inclusive);
            return _term;
        }

        public override string ToString()
        {
            var term = _term;
            while (term.Owner != null)
            {
                term = term.Owner;
            }

            return (string.IsNullOrWhiteSpace(_index) ? "" : _index + ".")
                + (string.IsNullOrWhiteSpace(_field) ? "" : _field + ":")
                + term;
        }
    }
}
