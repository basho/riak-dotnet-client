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

namespace CorrugatedIron.Models.Search
{
    public class BinaryTerm : Term
    {
        internal enum Op { And, Or }

        private readonly Term _left;
        private readonly Term _right;
        private readonly Op _op;
        private readonly Token _value;

        internal BinaryTerm(RiakFluentSearch search, string field, Op op, Term left, string value)
            : this(search, field, op, left, Token.Is(value))
        {
        }

        internal BinaryTerm(RiakFluentSearch search, string field, Op op, Term left, Token value)
            : this(search, field, op, left)
        {
            _value = value;
        }

        internal BinaryTerm(RiakFluentSearch search, string field, Op op, Term left, Term right)
            : this(search, field, op, left)
        {
            _right = right;
        }

        private BinaryTerm(RiakFluentSearch search, string field, Op op, Term left)
            : base(search, field)
        {
            _op = op;
            _left = left;
            left.Owner = this;
        }

        public override string ToString()
        {
            return _left + " " + _op.ToString().ToUpper() + " "
                + Prefix()
                + (_right != null ? _right.ToString() : Field() + _value) + Suffix();
        }
    }
}