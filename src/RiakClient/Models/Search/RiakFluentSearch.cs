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

    public class RiakFluentSearch
    {
        private readonly string index;
        private readonly string field;
        private Term term;
        private bool grouped;

        public RiakFluentSearch(string index, string field)
        {
            if (string.IsNullOrWhiteSpace(index))
            {
                throw new ArgumentNullException("index");
            }

            if (string.IsNullOrWhiteSpace(field))
            {
                throw new ArgumentNullException("field");
            }

            this.index = index;
            this.field = field;
        }

        [Obsolete("Bucket is deprecated, please use Index instead.", true)]
        public string Bucket
        {
            get { return index; }
        }

        public string Index
        {
            get { return index; }
        }

        public Term Search(string value)
        {
            return Search(Token.Is(value));
        }

        public Term Search(Token value)
        {
            this.term = new UnaryTerm(this, field, value);
            return this.term;
        }

        public Term Group(string value, Func<Term, Term> groupSetup)
        {
            return Group(Token.Is(value), groupSetup);
        }

        public Term Group(Token value, Func<Term, Term> groupSetup)
        {
            var groupedTerm = groupSetup(new UnaryTerm(this, field, value));
            grouped = true;
            this.term = new GroupTerm(this, field, groupedTerm);
            return this.term;
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
            this.term = new RangeTerm(this, field, from, to, inclusive);
            return this.term;
        }

        public ProximityTerm Proximity(int proximity, params string[] words)
        {
            return Proximity(field, proximity, words);
        }

        public ProximityTerm Proximity(string field, int proximity, params string[] words)
        {
            var term = new ProximityTerm(this, field, proximity, words);
            this.term = term;
            return term;
        }

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
