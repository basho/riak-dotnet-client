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
using System.Collections.Generic;
using CorrugatedIron.Models.MapReduce.KeyFilters;

namespace CorrugatedIron.Models.MapReduce.Fluent
{
    // TODO: Add summary info to public methods
    // TODO: Switch CorrugatedIron.Models.MapReduce.KeyFilters classes to internal
    public class RiakFluentKeyFilter
    {
        private IRiakKeyFilterToken _filter;

        internal IRiakKeyFilterToken Filter
        {
            get { return _filter; }
            private set { _filter = value; }
        }
                                    
        internal RiakFluentKeyFilter(IRiakKeyFilterToken filter)
        {
            Filter = filter;
        }

        public RiakFluentKeyFilter() {}

        public RiakFluentKeyFilter And(Action<RiakFluentKeyFilter> left, Action<RiakFluentKeyFilter> right)
        {
            var leftFluent = new RiakFluentKeyFilter();
            left(leftFluent);

            var rightFluent = new RiakFluentKeyFilter();
            right(rightFluent);

            Filter = new And(leftFluent.Filter, rightFluent.Filter);

            return this;
        }

        public RiakFluentKeyFilter Between<T>(T first, T second, bool inclusive)
        {
            Filter = new Between<T>(first, second, inclusive);
            return this;
        }

        public RiakFluentKeyFilter EndsWith(string arg)
        {
            Filter = new EndsWith(arg);
            return this;
        }

        public RiakFluentKeyFilter Equal<T>(T arg)
        {
            Filter = new Equal<T>(arg);
            return this;
        }

        public RiakFluentKeyFilter FloatToString()
        {
            Filter = new FloatToString();
            return this;
        }

        public RiakFluentKeyFilter GreaterThan<T>(T arg)
        {
            Filter = new GreaterThan<T>(arg);
            return this;
        }

        public RiakFluentKeyFilter GreaterThanOrEqualTo<T>(T arg)
        {
            Filter = new GreaterThanOrEqualTo<T>(arg);
            return this;
        }

        public RiakFluentKeyFilter IntToString()
        {
            Filter = new IntToString();
            return this;
        }

        public RiakFluentKeyFilter LessThan<T>(T arg)
        {
            Filter = new LessThan<T>(arg);
            return this;
        }

        public RiakFluentKeyFilter LessThanOrEqualTo<T>(T arg)
        {
            Filter = new LessThanOrEqualTo<T>(arg);
            return this;
        }

        public RiakFluentKeyFilter Matches<T>(T arg)
        {
            Filter = new Matches<T>(arg);
            return this;
        }

        /// <summary>
        /// Negates the result of key-filter operations.
        /// </summary>
        public RiakFluentKeyFilter Not(Action<RiakFluentKeyFilter> setup)
        {
            var fluent = new RiakFluentKeyFilter();
            setup(fluent);
            Filter = new Not(fluent.Filter);
            
            return this;
        }

        public RiakFluentKeyFilter NotEqual<T>(T arg)
        {
            Filter = new NotEqual<T>(arg);
            return this;
        }

        public RiakFluentKeyFilter Or(Action<RiakFluentKeyFilter> left, Action<RiakFluentKeyFilter> right)
        {
            var leftFluent = new RiakFluentKeyFilter();
            left(leftFluent);

            var rightFluent = new RiakFluentKeyFilter();
            right(rightFluent);

            Filter = new Or(leftFluent.Filter, rightFluent.Filter);

            return this;
        }

        public RiakFluentKeyFilter SetMember<T>(List<T> arg)
        {
            Filter = new SetMember<T>(arg);
            return this;
        }

        public RiakFluentKeyFilter SimilarTo<T>(T arg, int distance)
        {
            Filter = new SimilarTo<T>(arg, distance);
            return this;
        }

        public RiakFluentKeyFilter StartsWith(string arg)
        {
            Filter = new StartsWith(arg);
            return this;
        }

        public RiakFluentKeyFilter StringToFloat()
        {
            Filter = new StringToFloat();
            return this;
        }

        public RiakFluentKeyFilter StringToInt()
        {
            Filter = new StringToInt();
            return this;
        }

        public RiakFluentKeyFilter Tokenize(string token, uint position)
        {
            Filter = new Tokenize(token, position);
            return this;
        }

        public RiakFluentKeyFilter ToLower()
        {
            Filter = new ToLower();
            return this;
        }

        public RiakFluentKeyFilter ToUpper()
        {
            Filter = new ToUpper();
            return this;
        }

        public RiakFluentKeyFilter UrlDecode()
        {
            Filter = new UrlDecode();
            return this;
        }
    }
}
