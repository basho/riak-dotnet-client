// <copyright file="BinaryTerm.cs" company="Basho Technologies, Inc.">
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
    public class BinaryTerm : Term
    {
        private readonly Term left;
        private readonly Term right;
        private readonly Op op;
        private readonly Token value;

        internal BinaryTerm(RiakFluentSearch search, string field, Op op, Term left, string value)
            : this(search, field, op, left, Token.Is(value))
        {
        }

        internal BinaryTerm(RiakFluentSearch search, string field, Op op, Term left, Token value)
            : this(search, field, op, left)
        {
            this.value = value;
        }

        internal BinaryTerm(RiakFluentSearch search, string field, Op op, Term left, Term right)
            : this(search, field, op, left)
        {
            this.right = right;
        }

        private BinaryTerm(RiakFluentSearch search, string field, Op op, Term left)
            : base(search, field)
        {
            this.op = op;
            this.left = left;
            left.Owner = this;
        }

        internal enum Op
        {
            And,
            Or
        }

        public override string ToString()
        {
            return left + " " + op.ToString().ToUpper() + " "
                + Prefix()
                + (right != null ? right.ToString() : Field() + value) + Suffix();
        }
    }
}
