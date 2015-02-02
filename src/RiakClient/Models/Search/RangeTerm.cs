// <copyright file="RangeTerm.cs" company="Basho Technologies, Inc.">
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
    public class RangeTerm : Term
    {
        private readonly Token from;
        private readonly Token to;
        private readonly bool inclusive;

        public RangeTerm(RiakFluentSearch search, string field, Token from, Token to, bool inclusive)
            : base(search, field)
        {
            this.from = from;
            this.to = to;
            this.inclusive = inclusive;
        }

        public override string ToString()
        {
            var brackets = inclusive ? new[] { "[", "]" } : new[] { "{", "}" };
            return Prefix() + Field() + brackets[0] + from + " TO " + to + brackets[1];
        }
    }
}
