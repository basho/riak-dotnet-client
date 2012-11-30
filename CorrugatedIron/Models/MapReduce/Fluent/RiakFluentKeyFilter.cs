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

using CorrugatedIron.Models.MapReduce.KeyFilters;
using System;
using System.Collections.Generic;

namespace CorrugatedIron.Models.MapReduce.Fluent
{
    // TODO: Add summary info to public methods
    // TODO: Switch CorrugatedIron.Models.MapReduce.KeyFilters classes to internal
    public class RiakFluentKeyFilter
    {
        private readonly List<IRiakKeyFilterToken> _filters;

        internal RiakFluentKeyFilter(List<IRiakKeyFilterToken> filters)
        {
            _filters = filters;
        }

        public RiakFluentKeyFilter And(Action<RiakFluentKeyFilter> left, Action<RiakFluentKeyFilter> right)
        {
            var leftFilters = new List<IRiakKeyFilterToken>();
            var leftfluent = new RiakFluentKeyFilter(leftFilters);
            left(leftfluent);

            var rightFilters = new List<IRiakKeyFilterToken>();
            var rightFluent = new RiakFluentKeyFilter(rightFilters);
            right(rightFluent);

            _filters.Add(new And(leftFilters, rightFilters));

            return this;
        }

        public RiakFluentKeyFilter Between<T>(T first, T second, bool inclusive)
        {
            _filters.Add(new Between<T>(first, second, inclusive));
            return this;
        }

        public RiakFluentKeyFilter EndsWith(string arg)
        {
            _filters.Add(new EndsWith(arg));
            return this;
        }

        public RiakFluentKeyFilter Equal<T>(T arg)
        {
            _filters.Add(new Equal<T>(arg));
            return this;
        }

        public RiakFluentKeyFilter FloatToString()
        {
            _filters.Add(new FloatToString());
            return this;
        }

        public RiakFluentKeyFilter GreaterThan<T>(T arg)
        {
            _filters.Add(new GreaterThan<T>(arg));
            return this;
        }

        public RiakFluentKeyFilter GreaterThanOrEqualTo<T>(T arg)
        {
            _filters.Add(new GreaterThanOrEqualTo<T>(arg));
            return this;
        }

        public RiakFluentKeyFilter IntToString()
        {
            _filters.Add(new IntToString());
            return this;
        }

        public RiakFluentKeyFilter LessThan<T>(T arg)
        {
            _filters.Add(new LessThan<T>(arg));
            return this;
        }

        public RiakFluentKeyFilter LessThanOrEqualTo<T>(T arg)
        {
            _filters.Add(new LessThanOrEqualTo<T>(arg));
            return this;
        }

        public RiakFluentKeyFilter Matches(string arg)
        {
            _filters.Add(new Matches(arg));
            return this;
        }

        /// <summary>
        /// Negates the result of key-filter operations.
        /// </summary>
        public RiakFluentKeyFilter Not(Action<RiakFluentKeyFilter> setup)
        {
            var filters = new List<IRiakKeyFilterToken>();
            var fluent = new RiakFluentKeyFilter(filters);
            setup(fluent);
            _filters.Add(new Not(filters));

            return this;
        }

        public RiakFluentKeyFilter NotEqual<T>(T arg)
        {
            _filters.Add(new NotEqual<T>(arg));
            return this;
        }

        public RiakFluentKeyFilter Or(Action<RiakFluentKeyFilter> left, Action<RiakFluentKeyFilter> right)
        {
            var leftFilters = new List<IRiakKeyFilterToken>();
            var leftfluent = new RiakFluentKeyFilter(leftFilters);
            left(leftfluent);

            var rightFilters = new List<IRiakKeyFilterToken>();
            var rightFluent = new RiakFluentKeyFilter(rightFilters);
            right(rightFluent);

            _filters.Add(new Or(leftFilters, rightFilters));

            return this;
        }

        public RiakFluentKeyFilter SetMember<T>(List<T> arg)
        {
            _filters.Add(new SetMember<T>(arg));
            return this;
        }

        public RiakFluentKeyFilter SimilarTo<T>(T arg, int distance)
        {
            _filters.Add(new SimilarTo<T>(arg, distance));
            return this;
        }

        public RiakFluentKeyFilter StartsWith(string arg)
        {
            _filters.Add(new StartsWith(arg));
            return this;
        }

        public RiakFluentKeyFilter StringToFloat()
        {
            _filters.Add(new StringToFloat());
            return this;
        }

        public RiakFluentKeyFilter StringToInt()
        {
            _filters.Add(new StringToInt());
            return this;
        }

        public RiakFluentKeyFilter Tokenize(string token, uint position)
        {
            _filters.Add(new Tokenize(token, position));
            return this;
        }

        public RiakFluentKeyFilter ToLower()
        {
            _filters.Add(new ToLower());
            return this;
        }

        public RiakFluentKeyFilter ToUpper()
        {
            _filters.Add(new ToUpper());
            return this;
        }

        public RiakFluentKeyFilter UrlDecode()
        {
            _filters.Add(new UrlDecode());
            return this;
        }
    }
}
